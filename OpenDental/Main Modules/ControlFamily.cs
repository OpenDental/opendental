using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.Bridges;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{

	///<summary></summary>
	public partial class ControlFamily : UserControl {
		#region Fields - Public
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		#endregion Fields - Public

		#region Fields - Private
		private PatField[] _arrayPatFields;
		///<summary>Filled with all clones for the currently selected patient and their corresponding specialty.
		///Specialties are only important if clinics are enabled.  If clinics are disabled then the corresponding Def will be null.</summary>
		private Dictionary<Patient,Def> _dictCloneSpecialty;
		private DiscountPlanSub _discountPlanSub;
		private Family _family;
		private bool _initializedOnStartup;
		private List <Benefit> _listBenefits;
		private List <InsPlan> _listInsPlans;
		private List<InsSub> _listInsSubs;
		private List <PatPlan> _listPatPlans;
		private List<Patient> _listPatientsSuperFamilyGuarantors;
		private List<Patient> _listPatientsSuperFamilyMembers;
		///<summary>All recalls for this entire family.</summary>
		private List<Recall> _listRecalls;
		///<summary>All the data necessary to load the module.</summary>
		private FamilyModules.LoadData _loadData;
		private Patient _patient;
		private PatientNote _patientNote;
		///<summary>Gets updated to PatCur.PatNum that the last security log was made with so that we don't make too many security logs for this patient.  When _patNumLast no longer matches PatCur.PatNum (e.g. switched to a different patient within a module), a security log will be entered.  Gets reset (cleared and the set back to PatCur.PatNum) any time a module button is clicked which will cause another security log to be entered.</summary>
		private long _patNumLast;
		///<summary>Used for MenuItemPopup() to tell which row the user clicked on.  Currently only for gridPat</summary>
		private Point _pointLastClicked;
		private SortStrategy _sortStrategySuperFam;
		private ToolBarOD toolBarMain;
		#endregion Fields - Private

		#region Constructor
		///<summary></summary>
		public ControlFamily(){
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.Start);
			InitializeComponent();// This call is required by the Windows.Forms Form Designer.
			Font=LayoutManagerForms.FontInitial;
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.End);
		}
		#endregion Constructor

		#region Methods - Event Handlers - GridPatient
		private void gridPat_CellClick(object sender,ODGridClickEventArgs e) {
			GridCell gridCell=gridPat.ListGridRows[e.Row].Cells[e.Col];
			//Only grid cells with phone numbers are blue and underlined. 
			//If we support color and underline in the future, this might be changed to a regex of the cell text.
			if(gridCell.ColorText==System.Drawing.Color.Blue && gridCell.Underline==YN.Yes && Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
				DentalTek.PlaceCall(gridCell.Text);
			}
		}

		private void gridPat_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(Plugins.HookMethod(this,"ContrFamily.gridPat_CellDoubleClick",_patient)) {
				return;
			}
			if(TerminalActives.PatIsInUse(_patient.PatNum)) {
				MsgBox.Show(this,"Patient is currently entering info at a reception terminal.  Please try again later.");
				return;
			}
			if(gridPat.ListGridRows[e.Row].Tag==null 
				|| gridPat.ListGridRows[e.Row].Tag.ToString()=="SS#"
				|| gridPat.ListGridRows[e.Row].Tag.ToString()=="DOB") 
			{
				if(!Security.IsAuthorized(EnumPermType.PatientEdit)) {
					return;
				}
				using FormPatientEdit formPatientEdit=new FormPatientEdit(_patient,_family);
				formPatientEdit.IsNew=false;
				formPatientEdit.ShowDialog();
				if(formPatientEdit.DialogResult==DialogResult.OK) {
					FormOpenDental.S_Contr_PatientSelected(_patient,false);
				}
			}
			//Check tags and perform corresponding action for said tag type.
			else if(gridPat.ListGridRows[e.Row].Tag.ToString()=="Referral") {
				//RefAttach refattach=(RefAttach)gridPat.Rows[e.Row].Tag;
				using FormReferralsPatient formReferralsPatient=new FormReferralsPatient();
				formReferralsPatient.PatNum=_patient.PatNum;
				formReferralsPatient.ShowDialog();
			}
			else if(gridPat.ListGridRows[e.Row].Tag.ToString()=="References") {
				using FormReference formReference=new FormReference();
				formReference.ShowDialog();
				if(formReference.PatNumGoto!=0) {
					Patient patient=Patients.GetPat(formReference.PatNumGoto);
					FormOpenDental.S_Contr_PatientSelected(patient,false);
					GotoModule.GotoFamily(formReference.PatNumGoto);
					return;
				}
				if(formReference.DialogResult!=DialogResult.OK) {
					return;
				}
				for(int i=0;i<formReference.ListCustReferencesSelected.Count;i++) {
					CustRefEntry custRefEntry=new CustRefEntry();
					custRefEntry.DateEntry=DateTime.Now;
					custRefEntry.PatNumCust=_patient.PatNum;
					custRefEntry.PatNumRef=formReference.ListCustReferencesSelected[i].PatNum;
					CustRefEntries.Insert(custRefEntry);
				}
			}
			else if(gridPat.ListGridRows[e.Row].Tag.GetType()==typeof(CustRefEntry)) {
				using FormReferenceEntryEdit formReferenceEntryEdit=new FormReferenceEntryEdit((CustRefEntry)gridPat.ListGridRows[e.Row].Tag);
				formReferenceEntryEdit.ShowDialog();
			}
			else if(gridPat.ListGridRows[e.Row].Tag.ToString().Equals("Payor Types")) {
				using FormPayorTypes formPayorTypes=new FormPayorTypes();
				formPayorTypes.PatientCur=_patient;
				formPayorTypes.ShowDialog();
			}
			else if(gridPat.ListGridRows[e.Row].Tag is PatFieldDef) {//patfield for an existing PatFieldDef
				PatFieldDef patFieldDef=(PatFieldDef)gridPat.ListGridRows[e.Row].Tag;
				PatField patField=PatFields.GetByName(patFieldDef.FieldName,_arrayPatFields);
				PatFieldL.OpenPatField(patField,patFieldDef,_patient.PatNum);
			}
			else if(gridPat.ListGridRows[e.Row].Tag is PatField) {//PatField for a PatFieldDef that no longer exists
				PatField patField=(PatField)gridPat.ListGridRows[e.Row].Tag;
				using FormPatFieldEdit formPatFieldEdit=new FormPatFieldEdit(patField);
				formPatFieldEdit.ShowDialog();
			}
			else if(gridPat.ListGridRows[e.Row].Tag is Address) {
				Address address=(Address)gridPat.ListGridRows[e.Row].Tag;
				if(address.IsNew) { //add the patCur's patNum is new
					address.PatNumTaxPhysical=_patient.PatNum;
				}
				using FormTaxAddress formTaxAddress=new FormTaxAddress();
				formTaxAddress.AddressCur=address;
				formTaxAddress.PatCur=_patient;
				formTaxAddress.ShowDialog();
			}
			ModuleSelected(_patient.PatNum);
		}

		private void gridPat_MouseDown(object sender,MouseEventArgs e) {
			_pointLastClicked=e.Location;
		}

		///<summary>Just prior to displaying the context menu, enable or disables the UnmaskDOB option</summary>
		private void MenuItemPopupUnmaskDOB(object sender,EventArgs e) {
			MenuItem menuItemDOB=gridPat.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Name=="ViewDOB");
			if(menuItemDOB==null) { 
				return;//Should not happen
			}
			MenuItem menuItemSeperator=gridPat.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Text=="-");
			if(menuItemSeperator==null) { 
				return;//Should not happen
			}
			int idxRowClick=gridPat.PointToRow(_pointLastClicked.Y);
			int idxColClick=gridPat.PointToCol(_pointLastClicked.X);//Make sure the user clicked within the bounds of the grid.
			if(idxRowClick>-1 && idxColClick>-1 && (gridPat.ListGridRows[idxRowClick].Tag!=null)
				&& gridPat.ListGridRows[idxRowClick].Tag is string
				&& ((string)gridPat.ListGridRows[idxRowClick].Tag=="DOB"))
			{
				if(Security.IsAuthorized(EnumPermType.PatientDOBView,true)
					&& gridPat.ListGridRows[idxRowClick].Cells[gridPat.ListGridRows[idxRowClick].Cells.Count-1].Text!="")
				{
					menuItemDOB.Visible=true;
					menuItemDOB.Enabled=true;
				}
				else {
					menuItemDOB.Visible=true;
					menuItemDOB.Enabled=false;
				}
				menuItemSeperator.Visible=true;
				menuItemSeperator.Enabled=true;
				return;
			}
			menuItemDOB.Visible=false;
			menuItemDOB.Enabled=false;
			if(gridPat.ContextMenu.MenuItems.OfType<MenuItem>().Count(x => x.Visible==true && x.Text!="-")>1) {
				//There is more than one item showing, we want the seperator.
				menuItemSeperator.Visible=true;
				menuItemSeperator.Enabled=true;
				return;
			}
			//We dont want the seperator to be there with only one option.
			menuItemSeperator.Visible=false;
			menuItemSeperator.Enabled=false;
		}

		///<summary>Just prior to displaying the context menu, enable or disables the UnmaskSSN option</summary>
		private void MenuItemPopupUnmaskSSN(object sender,EventArgs e) {
			MenuItem menuItemSSN=gridPat.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Name=="ViewSS#");
			if(menuItemSSN==null) { 
				return;//Should not happen
			}
			MenuItem menuItemSeperator=gridPat.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Text=="-");
			if(menuItemSeperator==null) {
				return;//Should not happen
			}
			int idxRowClick=gridPat.PointToRow(_pointLastClicked.Y);
			int idxColClick=gridPat.PointToCol(_pointLastClicked.X);//Make sure the user clicked within the bounds of the grid.
			if(idxRowClick>-1 && idxColClick>-1 && (gridPat.ListGridRows[idxRowClick].Tag!=null) 
				&& gridPat.ListGridRows[idxRowClick].Tag is string
				&& ((string)gridPat.ListGridRows[idxRowClick].Tag=="SS#"))
			{
				if(Security.IsAuthorized(EnumPermType.PatientSSNView,true) 
					&& gridPat.ListGridRows[idxRowClick].Cells[gridPat.ListGridRows[idxRowClick].Cells.Count-1].Text!="")
				{
					menuItemSSN.Visible=true;
					menuItemSSN.Enabled=true;
				}
				else {
					menuItemSSN.Visible=true;
					menuItemSSN.Enabled=false;
				}
				menuItemSeperator.Visible=true;
				menuItemSeperator.Enabled=true;
				return;
			}
			menuItemSSN.Visible=false;
			menuItemSSN.Enabled=false;
			if(gridPat.ContextMenu.MenuItems.OfType<MenuItem>().Count(x => x.Visible==true && x.Text!="-")>1) {
				//There is more than one item showing, we want the seperator.
				menuItemSeperator.Visible=true;
				menuItemSeperator.Enabled=true;
				return;
			}
			//We dont want the seperator to be there with only one option.
			menuItemSeperator.Visible=false;
			menuItemSeperator.Enabled=false;
		}

		private void MenuItemUnmaskDOB_Click(object sender,EventArgs e) {
			//Preference and permissions check has already happened by this point.
			//Guaranteed to be clicking on a valid row & column.
			int rowClick=gridPat.PointToRow(_pointLastClicked.Y);
			gridPat.BeginUpdate();
			GridRow row=gridPat.ListGridRows[rowClick];
			row.Cells[row.Cells.Count-1].Text=Patients.DOBFormatHelper(_patient.Birthdate,false);
			gridPat.EndUpdate();
			string logtext="Date of birth unmasked in Family Module";
			SecurityLogs.MakeLogEntry(EnumPermType.PatientDOBView,_patient.PatNum,logtext);
		}

		private void MenuItemUnmaskSSN_Click(object sender,EventArgs e) {
			//Preference and permissions check has already happened by this point.
			//Guaranteed to be clicking on a valid row & column.
			int rowClick=gridPat.PointToRow(_pointLastClicked.Y);
			gridPat.BeginUpdate();
			GridRow row=gridPat.ListGridRows[rowClick];
			row.Cells[row.Cells.Count-1].Text=Patients.SSNFormatHelper(_patient.SSN,false);
			gridPat.EndUpdate();
			string logtext="";
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				logtext="Social Insurance Number";
			}
			else {
				logtext="Social Security Number";
			}
			logtext+=" unmasked in Family Module";
			SecurityLogs.MakeLogEntry(EnumPermType.PatientSSNView,_patient.PatNum,logtext);
		}
		#endregion Methods - Event Handlers - GridPatient 

		#region Methods - Event Handlers - GridFamily
		private void gridFamily_CellClick(object sender,ODGridClickEventArgs e) {
			FormOpenDental.S_Contr_PatientSelected(gridFamily.SelectedTag<Patient>(),false);
			ModuleSelected(gridFamily.SelectedTag<Patient>().PatNum);
		}

		private void gridFamily_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.PatientEdit)) {
				return;
			}
			using FormPatientEdit formPatientEdit=new FormPatientEdit(_patient,_family);
			formPatientEdit.IsNew=false;
			formPatientEdit.ShowDialog();
			if(formPatientEdit.DialogResult==DialogResult.OK) {
				FormOpenDental.S_Contr_PatientSelected(_patient,false);
			}
			ModuleSelected(_patient.PatNum);
		}
		#endregion Methods - Event Handlers - GridFamily

		#region Methods - Event Handlers - GridRecall
		private void gridRecall_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//use doubleclick instead
		}

		private void gridRecall_DoubleClick(object sender,EventArgs e) {
			if(_patient==null) {
				return;
			}
			using FormRecallsPat formRecallsPat=new FormRecallsPat();
			formRecallsPat.PatNum=_patient.PatNum;
			formRecallsPat.ShowDialog();
			ModuleSelected(_patient.PatNum);
		}
		#endregion Methods - Event Handlers - GridRecall

		#region Methods - Event Handlers - GridSuperFam
		private void gridSuperFam_CellClick(object sender,ODGridClickEventArgs e) {
			FormOpenDental.S_Contr_PatientSelected(_listPatientsSuperFamilyGuarantors[e.Row],false);
			ModuleSelected(_listPatientsSuperFamilyGuarantors[e.Row].PatNum);
		}

		private void gridSuperFam_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//OnPatientSelected(SuperFamilyGuarantors[e.Row].PatNum,SuperFamilyGuarantors[e.Row].GetNameLF(),SuperFamilyGuarantors[e.Row].Email!="",
			//  SuperFamilyGuarantors[e.Row].ChartNumber);
			//ModuleSelected(SuperFamilyGuarantors[e.Row].PatNum);
		}
		#endregion Methods - Event Handlers - GridSuperFam

		#region Methods - Event Handlers - GridIns
		private void gridIns_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//Handle if the patient has a discount plan
			if(_discountPlanSub!=null) {
				DiscountPlanSub discountPlanSub=DiscountPlanSubs.GetSubForPat(_patient.PatNum);
				if(discountPlanSub==null) {
					MsgBox.Show(this,"Discount plan removed by another user.");
					ModuleSelected(_patient.PatNum);
					return;
				}
				DiscountPlan discountPlan=DiscountPlans.GetPlan(discountPlanSub.DiscountPlanNum);
				if(discountPlan==null) {
					MsgBox.Show(this,"Discount plan deleted by another user.");
					ModuleSelected(_patient.PatNum);
					return;
				}
				if(discountPlan.DiscountPlanNum!=_discountPlanSub.DiscountPlanNum) {
					MsgBox.Show(this,"Discount plan changed by another user.");
					ModuleSelected(_patient.PatNum);
					return;
				}
				using FormDiscountPlanSubEdit formDiscountPlanSubEdit=new FormDiscountPlanSubEdit();
				formDiscountPlanSubEdit.DiscountPlanSubCur=discountPlanSub;
				if(formDiscountPlanSubEdit.ShowDialog()!=DialogResult.OK) {
					return;
				}
				_discountPlanSub=formDiscountPlanSubEdit.DiscountPlanSubCur;
				if(_discountPlanSub==null) {
					TreatPlans.UpdateTreatmentPlanType(_patient);
				}
				ModuleSelected(_patient.PatNum);
				return;
			}
			if(e.Col==0) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			//Retrieving information from database due to concurrency issues causing the Family Module to display an insurance plan that has potentially changed.
			PatPlan patPlan=PatPlans.GetByPatPlanNum(_listPatPlans[e.Col-1].PatPlanNum);
			if(patPlan==null) {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Insurance plan for this patient no longer exists.  Refresh the module.");
				return;
			}
			_listInsSubs=InsSubs.RefreshForFam(_family);
			_listInsPlans=InsPlans.RefreshForSubList(_listInsSubs);//this is only here in case, if in FormModuleSetup, the InsDefaultCobRule is changed and cob changed for all plans.
			InsSub insSub=_listInsSubs.Find(x => x.InsSubNum==patPlan.InsSubNum);
			InsPlan insPlan=InsPlans.GetPlan(insSub.PlanNum,_listInsPlans);
			string insHistPref=(string)((GridOD)sender).ListGridRows[e.Row].Tag;
			if(string.IsNullOrEmpty(insHistPref)) {
				Cursor=Cursors.Default;
				using FormInsPlan formInsPlan=new FormInsPlan(insPlan,patPlan,insSub);
				if(formInsPlan.ShowDialog()==DialogResult.OK) {
					if(_listPatPlans.Count(x => x.Ordinal==patPlan.Ordinal)>1) {
						UpdateConflictingOrdinals(patPlan);
					}
				}
			}
			else {
				Cursor=Cursors.Default;
				using FormInsHistSetup formInsHistSetup=new FormInsHistSetup(patPlan.PatNum,insSub);
				formInsHistSetup.ShowDialog();
			}
			Cursor=Cursors.Default;
			//Module is refreshed to reflect what the most recent information in the database is, but the module doesn't refresh if the insurance plan is edited by someone else.
			ModuleSelected(_patient.PatNum);//Should refresh insplans to display new information
		}

		private void menuItemRemoveDiscount_Click(object sender,EventArgs e) {
			if(_discountPlanSub==null) {
				FillInsData();
				return;
			}
			DiscountPlanSubs.UpdateAssociatedDiscountPlanAmts(new List<DiscountPlanSub>{ _discountPlanSub },true);
			if(!string.IsNullOrWhiteSpace(_discountPlanSub.SubNote) && MsgBox.Show(MsgBoxButtons.YesNo,Lan.g("Commlogs","Save Subscriber Note to Commlog?"))) {
				Commlog commlog=new Commlog();
				commlog.PatNum=_discountPlanSub.PatNum;
				commlog.CommDateTime=DateTime.Now;
				commlog.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.MISC);
				commlog.Note=Lans.g("Commlogs","Subscriber note from dropped discount plan, saved copy")+": ";
				commlog.Note+=_discountPlanSub.SubNote;
				commlog.UserNum=Security.CurUser.UserNum;
				Commlogs.Insert(commlog);
			}
			DiscountPlanSubs.Delete(_discountPlanSub.DiscountSubNum);
			string logText="The discount plan "+DiscountPlans.GetPlan(_discountPlanSub.DiscountPlanNum).Description+" was dropped.";
			SecurityLogs.MakeLogEntry(EnumPermType.DiscountPlanAddDrop,_patient.PatNum,logText);
			_discountPlanSub=null;
			FillInsData();
		}

		private void menuPlansForFam_Click(object sender,EventArgs e) {
			using FormPlansForFamily formPlansForFamily=new FormPlansForFamily();
			formPlansForFamily.FamilyCur=_family;
			formPlansForFamily.ShowDialog();
			ModuleSelected(_patient.PatNum);
		}
		#endregion Methods - Event Handlers - GridIns

		#region Methods - Event Handlers - Patient Clones
		private void gridPatientClone_CellClick(object sender,ODGridClickEventArgs e) {
			if(gridPatientClones.ListGridRows[e.Row].Tag==null || gridPatientClones.ListGridRows[e.Row].Tag.GetType()!=typeof(Patient)) {
				return;
			}
			Patient patient=(Patient)gridPatientClones.ListGridRows[e.Row].Tag;
			FormOpenDental.S_Contr_PatientSelected(patient,false);
			ModuleSelected(patient.PatNum);
		}
		#endregion Methods - Event Handlers - Patient Clones

		#region Methods - Event Handlers - Other
		private void ToolBarMain_ButtonClick(object sender,ODToolBarButtonClickEventArgs e) {
			if(e.Button.Tag.GetType()==typeof(string)) {
				//standard predefined button
				switch(e.Button.Tag.ToString()) {
					//case "Recall":
					//	ToolButRecall_Click();
					//	break;
					case "Add":
						ToolButAdd_Click();
						break;
					case "Delete":
						ToolButDelete_Click();
						break;
					case "Guarantor":
						ToolButGuarantor_Click();
						break;
					case "Move":
						ToolButMove_Click();
						break;
					case "Ins":
						ToolButIns_Click();
						break;
					case "Discount":
						ToolButDiscount_Click();
						break;
					case "AddSuper":
						ToolButAddSuper_Click();
						break;
					case "RemoveSuper":
						ToolButRemoveSuper_Click();
						break;
					case "DisbandSuper":
						ToolButDisbandSuper_Click();
						break;
					case "AddClone":
						ToolButAddClone_Click();
						break;
					case "SynchClone":
						ToolButSynchClone_Click();
						break;
					case "BreakClone":
						ToolButBreakClone_Click();
						break;
				}
				return;
			}
			if(e.Button.Tag.GetType()==typeof(Program)) {
				ProgramL.Execute(((Program)e.Button.Tag).ProgramNum,_patient);
			}
		}
		#endregion Methods - Event Handlers - Other

		#region Methods - Public
		///<summary></summary>
		public void InitializeOnStartup() {
			if(_initializedOnStartup) {
				return;
			}
			_initializedOnStartup=true;
			//tbFamily.InstantClasses();
			//cannot use Lan.F(this);
			Lan.C(this,new Control[]
				{
					//butPatEdit,
					//butEditPriCov,
					//butEditPriPlan,
					//butEditSecCov,
					//butEditSecPlan,
					gridFamily,
					gridRecall,
					gridPat,
					gridSuperFam,
					gridIns,
				});
			LayoutToolBar();
			//gridPat.Height=this.ClientRectangle.Bottom-gridPat.Top-2;
		}

		///<summary>Causes the toolbar to be laid out again.</summary>
		public void LayoutToolBar() {
			toolBarMain.Buttons.Clear();
			ODToolBarButton toolBarButton;
			//ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Recall"),1,"","Recall"));
			//ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarButton=new ODToolBarButton(Lan.g(this,"Family Members:"),-1,"","");
			toolBarButton.Style=ODToolBarButtonStyle.Label;
			toolBarMain.Buttons.Add(toolBarButton);
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Add"),EnumIcons.PatAdd,"Add Family Member","Add"));
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Delete"),EnumIcons.PatDelete,Lan.g(this,"Delete Family Member"),"Delete"));
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Set Guarantor"),EnumIcons.PatSetGuarantor,Lan.g(this,"Set as Guarantor"),"Guarantor"));
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Move"),EnumIcons.PatMoveFam,Lan.g(this,"Move to Another Family"),"Move"));
			if(PrefC.GetBool(PrefName.ShowFeaturePatientClone)) {
				toolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
				toolBarButton=new ODToolBarButton(Lan.g(this,"Clones:"),-1,"","");
				toolBarButton.Style=ODToolBarButtonStyle.Label;
				toolBarMain.Buttons.Add(toolBarButton);
				toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Add"),-1,Lan.g(this,"Creates a clone of the currently selected patient."),"AddClone"));
				toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Sync"),-1,Lan.g(this,"Sync information to the clone patient or create a clone of the currently selected patient if one does not exist"),"SynchClone"));
				toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Break"),-1,Lan.g(this,"Remove selected patient from the clone group."),"BreakClone"));
			}
			if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
				toolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
				toolBarButton=new ODToolBarButton(Lan.g(this,"Super Family:"),-1,"","");
				toolBarButton.Style=ODToolBarButtonStyle.Label;
				toolBarMain.Buttons.Add(toolBarButton);
				toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Add"),-1,"Add selected patient to a super family","AddSuper"));
				toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Remove"),-1,Lan.g(this,"Remove selected patient, and their family, from super family"),"RemoveSuper"));
				toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Disband"),-1,Lan.g(this,"Disband the current super family by removing all members of the super family."),"DisbandSuper"));
			}
			if(!PrefC.GetBool(PrefName.EasyHideInsurance)) {
				toolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
				toolBarButton=new ODToolBarButton(Lan.g(this,"Add Insurance"),6,"","Ins");
				toolBarButton.Style=ODToolBarButtonStyle.DropDownButton;
				toolBarButton.DropDownMenu=contextMenuInsurance;
				toolBarMain.Buttons.Add(toolBarButton);
				toolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
				toolBarButton=new ODToolBarButton(Lan.g(this,"Discount Plan"),-1,"","Discount");
				toolBarButton.Style=ODToolBarButtonStyle.DropDownButton;
				toolBarButton.DropDownMenu=contextMenuDiscount;
				toolBarMain.Buttons.Add(toolBarButton);
			}
			ProgramL.LoadToolbar(toolBarMain,EnumToolBar.FamilyModule);
			toolBarMain.Invalidate();
			Plugins.HookAddCode(this,"ContrFamily.LayoutToolBar_end",_patient);
		}

		///<summary></summary>
		public void ModuleSelected(long patNum) {
			RefreshModuleData(patNum);
			if(_patient!=null && _patient.PatStatus==PatientStatus.Deleted) {
				MsgBox.Show("Selected patient has been deleted by another workstation.");
				PatientL.RemoveFromMenu(_patient.PatNum);
				FormOpenDental.S_Contr_PatientSelected(new Patient(),false);
				RefreshModuleData(0);
			}
			if(_patient!=null && _patient.PatStatus==PatientStatus.Archived && !Security.IsAuthorized(EnumPermType.ArchivedPatientSelect,suppressMessage:true)) {
				FormOpenDental.S_Contr_PatientSelected(new Patient(),false);
				RefreshModuleData(0);
			}
			RefreshModuleScreen();
			PatientDashboardDataEvent.Fire(ODEventType.ModuleSelected,_loadData);
			if(_patient!=null && DatabaseIntegrities.DoShowPopup(_patient.PatNum,EnumModuleType.Family)) {
				bool areHashesValid=Patients.AreAllHashesValid(_patient,new List<Appointment>(),new List<PayPlan>(),new List<PaySplit>());
				if(!areHashesValid) {
					DatabaseIntegrities.AddPatientModuleToCache(_patient.PatNum,EnumModuleType.Family); //Add to cached list for next time
					//show popup
					DatabaseIntegrity databaseIntegrity=DatabaseIntegrities.GetModule();
					using FormDatabaseIntegrity formDatabaseIntegrity=new FormDatabaseIntegrity();
					formDatabaseIntegrity.MessageToShow=databaseIntegrity.Message;
					formDatabaseIntegrity.ShowDialog();
				}
			}
			Plugins.HookAddCode(this,"ContrFamily.ModuleSelected_end",patNum);
		}

		///<summary></summary>
		public void ModuleUnselected() {
			_family=null;
			_listInsPlans=null;
			_patNumLast=0;//Clear out the last pat num so that a security log gets entered that the module was "visited" or "refreshed".
			gridPat.ContextMenu=new ContextMenu();//This module is never really disposed. Get rid of any menu options we added, to avoid duplicates.
			Plugins.HookAddCode(this,"ContrFamily.ModuleUnselected_end");
		}
		#endregion Methods - Public

		#region Methods - Private - GridPatient
		private void FillPatientData() {
			if(_patient==null) {
				gridPat.BeginUpdate();
				gridPat.ListGridRows.Clear();
				gridPat.Columns.Clear();
				gridPat.EndUpdate();
				return;
			}
			if(PrefC.GetBool(PrefName.PatientSSNMasked)) {
				//Add "View SS#" right click option, MenuItemPopupUnmaskSSN will show and hide it as needed.
				if(gridPat.ContextMenu==null) {
					gridPat.ContextMenu=new ContextMenu();//ODGrid will automatically attach the defaut Popups
				}
				ContextMenu contextMenu=gridPat.ContextMenu;
				MenuItem menuItemUnmaskSSN=new MenuItem();
				menuItemUnmaskSSN.Enabled=false;
				menuItemUnmaskSSN.Visible=false;
				menuItemUnmaskSSN.Name="ViewSS#";
				menuItemUnmaskSSN.Text="View SS#";
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					menuItemUnmaskSSN.Text="View SIN";
				}
				menuItemUnmaskSSN.Click+= new System.EventHandler(this.MenuItemUnmaskSSN_Click);
				contextMenu.MenuItems.Add(menuItemUnmaskSSN);
				contextMenu.Popup+=MenuItemPopupUnmaskSSN;
			}
			if(PrefC.GetBool(PrefName.PatientDOBMasked)) {
				//Add "View DOB" right click option, MenuItemPopupUnmaskDOB will show and hide it as needed.
				if(gridPat.ContextMenu==null) {
					gridPat.ContextMenu=new ContextMenu();//ODGrid will automatically attach the defaut Popups
				}
				ContextMenu contextMenu=gridPat.ContextMenu;
				MenuItem menuItemUnmaskDOB=new MenuItem();
				menuItemUnmaskDOB.Enabled=false;
				menuItemUnmaskDOB.Visible=false;
				menuItemUnmaskDOB.Name="ViewDOB";
				menuItemUnmaskDOB.Text="View DOB";
				menuItemUnmaskDOB.Click+= new System.EventHandler(this.MenuItemUnmaskDOB_Click);
				contextMenu.MenuItems.Add(menuItemUnmaskDOB);
				contextMenu.Popup+=MenuItemPopupUnmaskDOB;
			}
			gridPat.BeginUpdate();
			gridPat.Columns.Clear();
			GridColumn col=new GridColumn("",100);
			gridPat.Columns.Add(col);
			col=new GridColumn("",150);
			gridPat.Columns.Add(col);
			gridPat.ListGridRows.Clear();
			GridRow row;
			List<DisplayField> listDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.PatientInformation);
			DisplayField displayField;
			List<Def> listDefsMiscColor=Defs.GetDefsForCategory(DefCat.MiscColors,true);
			EhrPatient ehrPatient=null;
			if(listDisplayFields.Exists(x=>x.InternalName=="DischargeDate")) {
				ehrPatient=EhrPatients.Refresh(_patient.PatNum);
			}
			for(int f=0;f<listDisplayFields.Count;f++) {
				displayField=listDisplayFields[f];
				row=new GridRow();
				#region Description Column
				if(displayField.Description=="") {
					if(displayField.InternalName=="SS#") {
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
							row.Cells.Add("SIN");
						}
						else if(CultureInfo.CurrentCulture.Name.Length>=4 && CultureInfo.CurrentCulture.Name.Substring(3)=="GB") {
							row.Cells.Add("");
						}
						else {
							row.Cells.Add("SS#");
						}
					}
					else if(displayField.InternalName=="State") {
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
							row.Cells.Add("Province");
						}
						else if(CultureInfo.CurrentCulture.Name.Length>=4 && CultureInfo.CurrentCulture.Name.Substring(3)=="GB") {
							row.Cells.Add("");
						}
						else {
							row.Cells.Add("State");
						}
					}
					else if(displayField.InternalName=="Zip") {
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
							row.Cells.Add("Postal Code");
						}
						else if(CultureInfo.CurrentCulture.Name.Length>=4 && CultureInfo.CurrentCulture.Name.Substring(3)=="GB") {
							row.Cells.Add("Postcode");
						}
						else {
							row.Cells.Add(Lan.g("TablePatient","Zip"));
						}
					}
					else if(displayField.InternalName=="PatFields") {
						//don't add a cell
					}
					else {
						row.Cells.Add(displayField.InternalName);
					}
				}
				else {
					if(displayField.InternalName=="PatFields") {
						//don't add a cell
					}
					else {
						row.Cells.Add(displayField.Description);
					}
				}
				#endregion Description Column
				#region Value Column
				switch(displayField.InternalName) {
					#region ABC0
					case "ABC0":
						row.Cells.Add(_patient.CreditType);
						break;
					#endregion ABC0
					#region Addr/Ph Note
					case "Addr/Ph Note":
						row.Cells.Add(_patient.AddrNote);
						if(_patient.AddrNote!="") {
							row.ColorText=Color.Red;
							row.Bold=true;
						}
						break;
					#endregion Addr/Ph Note
					#region Address
					case "Address":
						row.Cells.Add(_patient.Address);
						row.Bold=true;
						break;
					#endregion Address
					#region Address2
					case "Address2":
						row.Cells.Add(_patient.Address2);
						break;
					#endregion Address2
					#region AdmitDate
					case "AdmitDate":
						row.Cells.Add(_patient.AdmitDate.ToShortDateString());
						break;
					#endregion AdmitDate
					#region Age
					case "Age":
						row.Cells.Add(PatientLogic.DateToAgeString(_patient.Birthdate,_patient.DateTimeDeceased));
						break;
					#endregion Age
					#region Arrive Early
					case "Arrive Early":
						if(_patient.AskToArriveEarly==0) {
							row.Cells.Add("");
							break;
						}
						row.Cells.Add(_patient.AskToArriveEarly.ToString());
						break;
					#endregion Arrive Early
					#region Billing Type
					case "Billing Type":
						string billingtype=Defs.GetName(DefCat.BillingTypes,_patient.BillingType);
						if(Defs.GetHidden(DefCat.BillingTypes,_patient.BillingType)) {
							billingtype+=" "+Lan.g(this,"(hidden)");
						}
						row.Cells.Add(billingtype);
						break;
					#endregion Billing Type
					#region Birthdate
					case "Birthdate":
						if(PrefC.GetBool(PrefName.PatientDOBMasked) || !Security.IsAuthorized(EnumPermType.PatientDOBView,true)) {
							row.Cells.Add(Patients.DOBFormatHelper(_patient.Birthdate,true));
							row.Tag="DOB";//Used later to tell if we're right clicking on the DOB row
							break;
						}
						row.Cells.Add(Patients.DOBFormatHelper(_patient.Birthdate,false));
						break;
					#endregion Birthdate
					#region Chart Num
					case "Chart Num":
						row.Cells.Add(_patient.ChartNumber);
						break;
					#endregion Chart Num
					#region City
					case "City":
						row.Cells.Add(_patient.City);
						break;
					#endregion City
					#region Clinic
					case "Clinic":
						row.Cells.Add(Clinics.GetAbbr(_patient.ClinicNum));
						break;
					#endregion Clinic
					#region Contact Method
					case "Contact Method":
						row.Cells.Add(_patient.PreferContactMethod.ToString());
						if(_patient.PreferContactMethod==ContactMethod.DoNotCall || _patient.PreferContactMethod==ContactMethod.SeeNotes) {
							row.Bold=true;
						}
						break;
					#endregion Contact Method
					#region Country
					case "Country":
						row.Cells.Add(_patient.Country);
						break;
					#endregion Country
					#region DischargeDate
					case "DischargeDate":
						row.Cells.Add(ehrPatient.DischargeDate.ToShortDateString());
						break;
					#endregion DischargeDate
					#region E-mail
					case "E-mail":
						row.Cells.Add(_patient.Email);
						if(_patient.PreferContactMethod==ContactMethod.Email) {
							row.Bold=true;
						}
						break;
					#endregion E-mail
					#region First
					case "First":
						row.Cells.Add(_patient.FName);
						break;
					#endregion First
					#region Gender
					case "Gender":
						row.Cells.Add(_patient.Gender.ToString());
						break;
					#endregion Gender
					#region Guardians
					case "Guardians":
						List<Guardian> listGuardians=_loadData.ListGuardians??Guardians.Refresh(_patient.PatNum);
						string str="";
						for(int g=0;g<listGuardians.Count;g++) {
							if(!listGuardians[g].IsGuardian) {
								continue;
							}
							if(g>0) {
								str+=",";
							}
							str+=_family.GetNameInFamFirst(listGuardians[g].PatNumGuardian)+Guardians.GetGuardianRelationshipStr(listGuardians[g].Relationship);
						}
						row.Cells.Add(str);
						break;
					#endregion Guardians
					#region Hm Phone
					case "Hm Phone":
						row.Cells.Add(_patient.HmPhone);
						if(_patient.PreferContactMethod==ContactMethod.HmPhone || _patient.PreferContactMethod==ContactMethod.None) {
							row.Bold=true;
						}
						if(Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
							row.Cells[row.Cells.Count-1].ColorText=Color.Blue;
							row.Cells[row.Cells.Count-1].Underline=YN.Yes;
						}
						break;
					#endregion Hm Phone
					#region ICE Name
					case "ICE Name":
						row.Cells.Add(_patientNote.ICEName);
						row.ColorBackG=listDefsMiscColor[(int)DefCatMiscColors.FamilyModuleICE].ItemColor;
						break;
					#endregion ICE Name
					#region ICE Phone
					case "ICE Phone":
						row.Cells.Add(_patientNote.ICEPhone);
						row.ColorBackG=listDefsMiscColor[(int)DefCatMiscColors.FamilyModuleICE].ItemColor;
						break;
					#endregion ICE Phone
					#region Language
					case "Language":
						if(_patient.Language=="" || _patient.Language==null) {
							row.Cells.Add("");
							break;
						}
						try {
							row.Cells.Add(CodeBase.MiscUtils.GetCultureFromThreeLetter(_patient.Language).DisplayName);
							//row.Cells.Add(CultureInfo.GetCultureInfo(PatCur.Language).DisplayName);
						}
						catch {
							row.Cells.Add(_patient.Language);
						}
						break;
					#endregion Language
					#region Last
					case "Last":
						row.Cells.Add(_patient.LName);
						break;
					#endregion Last
					#region Middle
					case "Middle":
						row.Cells.Add(_patient.MiddleI);
						break;
					#endregion Middle
					#region PatFields
					case "PatFields":
						PatFieldL.AddPatFieldsToGrid(gridPat,_arrayPatFields.ToList(),FieldLocations.Family);
						break;
					#endregion PatFields
					#region Pat Restrictions
					case "Pat Restrictions":
						List<PatRestriction> listPatRestricts=_loadData.ListPatRestricts??PatRestrictions.GetAllForPat(_patient.PatNum);
						if(listPatRestricts.Count==0) {
							row.Cells.Add(Lan.g("TablePatient","None"));//row added outside of switch statement
						}
						for(int i=0;i<listPatRestricts.Count;i++) {
							row=new GridRow();
							if(string.IsNullOrWhiteSpace(displayField.Description)) {
								row.Cells.Add(displayField.InternalName);
							}
							else {
								row.Cells.Add(displayField.Description);
							}
							row.Cells.Add(PatRestrictions.GetPatRestrictDesc(listPatRestricts[i].PatRestrictType));
							row.ColorBackG=listDefsMiscColor[(int)DefCatMiscColors.FamilyModPatRestrict].ItemColor;//Patient Restrictions (hard coded in convertdatabase4)
							if(i==listPatRestricts.Count-1) {//last row added outside of switch statement
								break;
							}
							gridPat.ListGridRows.Add(row);
						}
						break;
					#endregion Pat Restrictions
					#region Payor Types
					case "Payor Types":
						row.Tag="Payor Types";
						row.Cells.Add(_loadData.PayorTypeDesc??PayorTypes.GetCurrentDescription(_patient.PatNum));
						break;
					#endregion Payor Types
					#region Position
					case "Position":
						row.Cells.Add(_patient.Position.ToString());
						break;
					#endregion Position
					#region Preferred
					case "Preferred":
						row.Cells.Add(_patient.Preferred);
						break;
					#endregion Preferred
					#region Preferred Pronoun
					case "Preferred Pronoun":
						row.Cells.Add(_patientNote.Pronoun.ToString());
						break;
					#endregion
					#region Primary Provider
					case "Primary Provider":
						if(_patient.PriProv!=0) {
							row.Cells.Add(Providers.GetLongDesc(Patients.GetProvNum(_patient)));
							break;
						}
						row.Cells.Add(Lan.g("TablePatient","None"));
						break;
					#endregion Primary Provider
					#region References
					case "References":
						List<CustRefEntry> custREList=_loadData.ListCustRefEntries??CustRefEntries.GetEntryListForCustomer(_patient.PatNum);
						if(custREList.Count==0) {
							row.Cells.Add(Lan.g("TablePatient","None"));
							row.Tag="References";
							row.ColorBackG=listDefsMiscColor[(int)DefCatMiscColors.FamilyModuleReferral].ItemColor;
						}
						else {
							row.Cells.Add(Lan.g("TablePatient",""));
							row.Tag="References";
							row.ColorBackG=listDefsMiscColor[(int)DefCatMiscColors.FamilyModuleReferral].ItemColor;
							gridPat.ListGridRows.Add(row);
						}
						for(int i=0;i<custREList.Count;i++) {
							row=new GridRow();
							if(custREList[i].PatNumRef==_patient.PatNum) {
								row.Cells.Add(custREList[i].DateEntry.ToShortDateString());
								row.Cells.Add("For: "+CustReferences.GetCustNameFL(custREList[i].PatNumCust));
							}
							else {
								row.Cells.Add("");
								row.Cells.Add(CustReferences.GetCustNameFL(custREList[i].PatNumRef));
							}
							row.Tag=custREList[i];
							row.ColorBackG=listDefsMiscColor[(int)DefCatMiscColors.FamilyModuleReferral].ItemColor;
							if(i<custREList.Count-1) {
								gridPat.ListGridRows.Add(row);
							}
						}
						break;
					#endregion References
					#region Referrals
					case "Referrals":
						List<RefAttach> listRefs=_loadData.ListRefAttaches??RefAttaches.Refresh(_patient.PatNum);
						List<RefAttach> listRefsFiltered= new List<RefAttach>();
						listRefsFiltered.AddRange(listRefs.Where(x => x.RefType==ReferralType.RefCustom).DistinctBy(x => x.ReferralNum).ToList());
						listRefsFiltered.AddRange(listRefs.Where(x => x.RefType==ReferralType.RefFrom).DistinctBy(x => x.ReferralNum).ToList());
						listRefsFiltered.AddRange(listRefs.Where(x => x.RefType==ReferralType.RefTo).DistinctBy(x => x.ReferralNum).ToList());
						listRefs=listRefsFiltered;
						if(listRefs.Count==0){
							row.Cells.Add(Lan.g("TablePatient","None"));
							row.Tag="Referral";
							row.ColorBackG=listDefsMiscColor[(int)DefCatMiscColors.FamilyModuleReferral].ItemColor;
						}
						//else{
						//	row.Cells.Add("");
						//	row.Tag="Referral";
						//	row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.FamilyModuleReferral].ItemColor;
						//}
						for(int i=0;i<listRefs.Count;i++) {
							row=new GridRow();
							if(listRefs[i].RefType==ReferralType.RefFrom){
								row.Cells.Add(Lan.g("TablePatient","Referred From"));
							}
							else if(listRefs[i].RefType==ReferralType.RefTo) {
								row.Cells.Add(Lan.g("TablePatient","Referred To"));
							}
							else {
								if(!string.IsNullOrWhiteSpace(displayField.Description)) {
									row.Cells.Add(displayField.Description);
								}
								else {
									row.Cells.Add(Lan.g("TablePatient","Referral"));
								}
							}
							Referral referral=Referrals.GetFromList(listRefs[i].ReferralNum);
							string refInfo=Referrals.GetNameLF(listRefs[i].ReferralNum);
							string phoneInfo=Referrals.GetPhone(listRefs[i].ReferralNum);
							if(!string.IsNullOrWhiteSpace(phoneInfo)) {
								refInfo+=$"\r\n{phoneInfo}";
							}
							if(referral==null) {
								row.Cells.Add("");//if referral is null because using random keys and had bug.
							}
							else {
								if(!string.IsNullOrWhiteSpace(referral.DisplayNote)) {
									refInfo+=$"\r\n{Lan.g("Referral","Display Note")}: {referral.DisplayNote}";
								}
								if(!string.IsNullOrWhiteSpace(listRefs[i].Note)) {
									refInfo+=$"\r\n{Lan.g("RefAttach","Patient Note")}: {listRefs[i].Note}";
								}
								row.Cells.Add(refInfo);
							}
							row.Tag="Referral";
							row.ColorBackG=listDefsMiscColor[(int)DefCatMiscColors.FamilyModuleReferral].ItemColor;
							if(i<listRefs.Count-1) {
								gridPat.ListGridRows.Add(row);
							}
						}
						break;
					#endregion Referrals
					#region ResponsParty
					case "ResponsParty":
						if(_patient.ResponsParty==0) {
							row.Cells.Add("");
						}
						else {
							row.Cells.Add((_loadData.ResponsibleParty??Patients.GetLim(_patient.ResponsParty)).GetNameLF());
						}
						row.ColorBackG=listDefsMiscColor[(int)DefCatMiscColors.FamilyModuleReferral].ItemColor;
						break;
					#endregion ResponsParty
					#region Salutation
					case "Salutation":
						row.Cells.Add(_patient.Salutation);
						break;
					#endregion Salutation
					#region Sec. Provider
					case "Sec. Provider":
						if(_patient.SecProv != 0) {
							row.Cells.Add(Providers.GetLongDesc(_patient.SecProv));
							break;
						}
						row.Cells.Add(Lan.g("TablePatient","None"));
						break;
					#endregion Sec. Provider
					#region SS#
					case "SS#":
						if(PrefC.GetBool(PrefName.PatientSSNMasked)) {
							row.Cells.Add(Patients.SSNFormatHelper(_patient.SSN,true));
							row.Tag="SS#";//Used later to tell if we're right clicking on the SSN row
							break;
						}
						row.Cells.Add(Patients.SSNFormatHelper(_patient.SSN,false));
						break;
					#endregion SS#
					#region State
					case "State":
						row.Cells.Add(_patient.State);
						break;
					#endregion State
					#region Status
					case "Status":
						row.Cells.Add(_patient.PatStatus.ToString());
						if(_patient.PatStatus==PatientStatus.Deceased) {
							row.ColorText=Color.Red;
						}
						break;
					#endregion Status
					#region Super Head
					case "Super Head":
						string fieldVal="";
						if(_patient.SuperFamily!=0) {
							Patient supHead=_loadData.SuperFamilyGuarantors.FirstOrDefault(x => x.PatNum==_patient.SuperFamily)??Patients.GetPat(_patient.SuperFamily);
							fieldVal=supHead.GetNameLF()+" ("+supHead.PatNum+")";
						}
						row.Cells.Add(fieldVal);
						break;
					#endregion Super Head
					#region Tax Address
					case "Tax Address":
						if (!PrefC.IsODHQ) {
							break;
						}
						row.Bold=true;
						Address address=Addresses.GetOneByPatNum(_patient.PatNum);//can be null
						row.Tag=address;
						//If the current customer doesn't have a tax address, don't display other fields
						if(address==null) {
							address=new Address();//need an address object in double click to identify row type
							address.IsNew=true;
							row.Cells.Add("");
							row.Tag=address;
							break;
						}
						string rowText=address.Address1;
						if (address.Address2!="") {
							rowText+="\r\n"+address.Address2;
						}
						rowText+="\r\n"+address.City+", "+address.State+" "+address.Zip;
						row.Cells.Add(rowText);
						break;
					#endregion Tax Address
					#region Title
					case "Title":
						row.Cells.Add(_patient.Title);
						break;
					#endregion Title
					#region Ward
					case "Ward":
						row.Cells.Add(_patient.Ward);
						break;
					#endregion Ward
					#region Wireless Ph
					case "Wireless Ph":
						row.Cells.Add(_patient.WirelessPhone);
						if(_patient.PreferContactMethod==ContactMethod.WirelessPh) {
							row.Bold=true;
						}
						if(Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
							row.Cells[row.Cells.Count-1].ColorText=Color.Blue;
							row.Cells[row.Cells.Count-1].Underline=YN.Yes;
						}
						break;
					#endregion Wireless Ph
					#region Wk Phone
					case "Wk Phone":
						row.Cells.Add(_patient.WkPhone);
						if(_patient.PreferContactMethod==ContactMethod.WkPhone) {
							row.Bold=true;
						}
						if(Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
							row.Cells[row.Cells.Count-1].ColorText=Color.Blue;
							row.Cells[row.Cells.Count-1].Underline=YN.Yes;
						}
						break;
					#endregion Wk Phone
					#region Zip
					case "Zip":
						row.Cells.Add(_patient.Zip);
						break;
					#endregion Zip
				}
				#endregion Value Column
				if(displayField.InternalName=="PatFields") {
					//don't add the row here
					continue;
				}
				gridPat.ListGridRows.Add(row);
			}
			gridPat.EndUpdate();
		}
		#endregion Methods - Private - GridPatient

		#region Methods - Private - GridFamily
		private void FillFamilyData() {
			gridFamily.BeginUpdate();
			gridFamily.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TablePatient","Name"),140);
			gridFamily.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePatient","Position"),65);
			gridFamily.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePatient","Gender"),55);
			gridFamily.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePatient","Status"),65);
			gridFamily.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePatient","Age"),45);
			gridFamily.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePatient","Recall Due"),80);
			gridFamily.Columns.Add(col);
			gridFamily.ListGridRows.Clear();
			if(_patient==null) {
				gridFamily.EndUpdate();
				return;
			}
			GridRow row;
			DateTime dateRecall;
			GridCell cell;
			int selectedRow=-1;
			for(int i=0;i<_family.ListPats.Length;i++) {
				if(PatientLinks.WasPatientMerged(_family.ListPats[i].PatNum,_loadData.ListMergeLinks) && _family.ListPats[i].PatNum!=_patient.PatNum) {
					//Hide merged patients so that new things don't get added to them. If the user really wants to find this patient, they will have to use 
					//the Select Patient window.
					continue;
				}
				if(_family.ListPats[i].PatStatus==PatientStatus.Deleted) {
					//Don't show deleted family members
					continue;
				}
				row=new GridRow();
				row.Cells.Add(_family.GetNameInFamLFI(i));
				row.Cells.Add(Lan.g("enumPatientPosition",_family.ListPats[i].Position.ToString()));
				row.Cells.Add(Lan.g("enumPatientGender",_family.ListPats[i].Gender.ToString()));
				row.Cells.Add(Lan.g("enumPatientStatus",_family.ListPats[i].PatStatus.ToString()));
				row.Cells.Add(PatientLogic.DateToAgeString(_family.ListPats[i].Birthdate,_family.ListPats[i].DateTimeDeceased));
				dateRecall=DateTime.MinValue;
				for(int j=0;j<_listRecalls.Count;j++) {
					if(_listRecalls[j].PatNum!=_family.ListPats[i].PatNum) {
						continue;
					}
					if(_listRecalls[j].RecallTypeNum==PrefC.GetLong(PrefName.RecallTypeSpecialProphy)) {
						dateRecall=_listRecalls[j].DateDue;
						continue;
					}
					if( _listRecalls[j].RecallTypeNum==PrefC.GetLong(PrefName.RecallTypeSpecialPerio)) {
						dateRecall=_listRecalls[j].DateDue;
					}
				}
				cell=new GridCell();
				if(dateRecall.Year>1880) {
					cell.Text=dateRecall.ToShortDateString();
					if(dateRecall<DateTime.Today) {
						cell.Bold=YN.Yes;
						cell.ColorText=Color.Firebrick;
					}
				}
				row.Cells.Add(cell);
				if(i==0){//guarantor
					row.Bold=true;
				}
				row.Tag=_family.ListPats[i];
				gridFamily.ListGridRows.Add(row);
				int idx=gridFamily.ListGridRows.Count-1;
				if(_family.ListPats[i].PatNum==_patient.PatNum) {
					selectedRow=idx;
				}
			}
			gridFamily.EndUpdate();
			gridFamily.SetSelected(selectedRow,true);
		}

		private void ToolButAdd_Click() {
			if(!Security.IsAuthorized(EnumPermType.PatientEdit)) {
				return;
			}
			//At HQ, some resellers don't add clients through the reseller portal.
			//Instead, they contact the conversions department and conversions creates a new account for them and adds them to the superfamily.
			//These accounts are acceptable to add because HQ understands they are not accounts designed to be managed by the Reseller Portal.
			Patient patientTemp=new Patient();
			patientTemp.LName         =_patient.LName;
			patientTemp.PatStatus     =PatientStatus.Deleted;
			patientTemp.Gender        =PatientGender.Unknown;
			patientTemp.Address       =_patient.Address;
			patientTemp.Address2      =_patient.Address2;
			patientTemp.City          =_patient.City;
			patientTemp.State         =_patient.State;
			patientTemp.Zip           =_patient.Zip;
			patientTemp.HmPhone       =_patient.HmPhone;
			patientTemp.WirelessPhone =_patient.WirelessPhone;
			patientTemp.WkPhone       =_patient.WkPhone;
			patientTemp.Email         =_patient.Email;
			patientTemp.TxtMsgOk      =_patient.TxtMsgOk;
			patientTemp.ShortCodeOptIn=_patient.ShortCodeOptIn;
			patientTemp.Guarantor     =_patient.Guarantor;
			patientTemp.CreditType    =_patient.CreditType;
			if(!PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
				patientTemp.PriProv     =_patient.PriProv;
			}
			patientTemp.SecProv       =_patient.SecProv;
			patientTemp.FeeSched      =_patient.FeeSched;
			patientTemp.BillingType   =_patient.BillingType;
			patientTemp.AddrNote      =_patient.AddrNote;
			patientTemp.ClinicNum     =_patient.ClinicNum;//this is probably better in case they don't have user.ClinicNums set.
			//tempPat.ClinicNum  =Security.CurUser.ClinicNum;
			if(Patients.GetPat(patientTemp.Guarantor).SuperFamily!=0) {
				patientTemp.SuperFamily=_patient.SuperFamily;
			}
			Patients.Insert(patientTemp,false);
			SecurityLogs.MakeLogEntry(EnumPermType.PatientCreate,patientTemp.PatNum,"Created from Family Module Add button.");
			CustReference custReference=new CustReference();
			custReference.PatNum=patientTemp.PatNum;
			CustReferences.Insert(custReference);
			//add the tempPat to the FamCur list, but ModuleSelected below will refill the FamCur list in case the user cancels and tempPat is deleted
			//This would be a faster way to add to the array, but since it is not a pattern that is used anywhere we will use the alternate method of
			//creating a list, adding the patient, and converting back to an array
			//Array.Resize(ref FamCur.ListPats,FamCur.ListPats.Length+1);
			//FamCur.ListPats[FamCur.ListPats.Length-1]=tempPat;
			//Adding the temp patient to the FamCur.ListPats without calling GetFamily which makes a call to the db
			List<Patient> listPatientsTemp=_family.ListPats.ToList();
			listPatientsTemp.Add(patientTemp);
			_family.ListPats=listPatientsTemp.ToArray();
			using FormPatientEdit formPatientEdit=new FormPatientEdit(patientTemp,_family);
			formPatientEdit.IsNew=true;
			formPatientEdit.ShowDialog();
			if(formPatientEdit.DialogResult==DialogResult.OK) {
				FormOpenDental.S_Contr_PatientSelected(patientTemp,false);
				ModuleSelected(patientTemp.PatNum);
				return;
			}
			ModuleSelected(_patient.PatNum);
		}

		private void ToolButDelete_Click() {
			if(!Security.IsAuthorized(EnumPermType.PatientEdit)) {
				return;
			}
			//this doesn't actually delete the patient, just changes their status
			//and they will never show again in the patient selection list.
			//check for plans, appointments, procedures, etc.
			List<Procedure> listProcedures=Procedures.Refresh(_patient.PatNum);
			List<Appointment> listAppointments=Appointments.GetPatientData(_patient.PatNum);
			List<Claim> listClaims=Claims.Refresh(_patient.PatNum);
			Adjustment[] adjustmentArray=Adjustments.Refresh(_patient.PatNum);
			PaySplit[] paySplitArray=PaySplits.Refresh(_patient.PatNum);//
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(_patient.PatNum);
			List<Commlog> listCommlogs=Commlogs.Refresh(_patient.PatNum);
			int countPayPlans=PayPlans.GetDependencyCount(_patient.PatNum);
			List<InsSub> listInsSubs=InsSubs.RefreshForFam(_family);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			List<MedicationPat> listMedicationPats=MedicationPats.Refresh(_patient.PatNum,false);
			_listPatPlans=PatPlans.Refresh(_patient.PatNum);
			//CovPats.Refresh(planList,PatPlanList);
			List<RefAttach> listRefAttaches=RefAttaches.Refresh(_patient.PatNum);
			List<Sheet> listSheets=Sheets.GetForPatient(_patient.PatNum);
			RepeatCharge[] repeatChargeArray=RepeatCharges.Refresh(_patient.PatNum);
			List<CreditCard> listCreditCards=CreditCards.Refresh(_patient.PatNum);
			RegistrationKey[] registrationKeyArray=RegistrationKeys.GetForPatient(_patient.PatNum);
			List<long> listPatNumClones=Patients.GetClonePatNumsAll(_patient.PatNum);
			long discountPlanNum=DiscountPlanSubs.GetDiscountPlanNumForPat(_patient.PatNum);
			bool hasProcs=listProcedures.Count>0;
			bool hasAppt=listAppointments.Count>0;
			bool hasClaims=listClaims.Count>0;
			bool hasAdj=adjustmentArray.Length>0;
			bool hasPay=paySplitArray.Length>0;
			bool hasClaimProcs=listClaimProcs.Count>0;
			bool hasComm=listCommlogs.Count>0;
			bool hasPayPlans=countPayPlans>0;
			//Has patplan or is a subscriber to an insplan.
			bool hasInsPlans=_listPatPlans.Count>0 || listInsSubs.Any(x => x.Subscriber==_patient.PatNum);
			bool hasMeds=listMedicationPats.Count>0;
			bool isSuperFamilyHead=_patient.PatNum==_patient.SuperFamily;
			bool hasRef=listRefAttaches.Count>0;
			bool hasSheets=listSheets.Count>0;
			bool hasRepeat=repeatChargeArray.Length>0;
			bool hasCC=listCreditCards.Count>0;
			bool hasRegKey=registrationKeyArray.Length>0;
			bool hasPerio=PerioExams.GetExamsTable(_patient.PatNum).Rows.Count>0;
			bool hasClones=(listPatNumClones.Count>1);//The list of "clones for all" will always include the current patient.
			bool hasDiscount=discountPlanNum>0;
			bool hasAllergies=Allergies.GetAll(_patient.PatNum,true).Any();
			bool hasProblems=Diseases.Refresh(true,_patient.PatNum).Any();
			if(hasProcs || hasAppt || hasClaims || hasAdj || hasPay || hasClaimProcs || hasComm || hasPayPlans || hasInsPlans
				|| hasRef || hasMeds || isSuperFamilyHead || hasSheets || hasRepeat || hasCC || hasRegKey || hasPerio || hasClones || hasDiscount || hasAllergies || hasProblems) 
			{
				string message=Lan.g(this,"You cannot delete this patient without first deleting the following data:")+"\r";
				if(hasProcs) {
					message+=Lan.g(this,"Procedures")+"\r";
				}
				if(hasAppt) {
					message+=Lan.g(this,"Appointments")+"\r";
				}
				if(hasClaims) {
					message+=Lan.g(this,"Claims")+"\r";
				}
				if(hasAdj) {
					message+=Lan.g(this,"Adjustments")+"\r";
				}
				if(hasPay) {
					message+=Lan.g(this,"Payments")+"\r";
				}
				if(hasClaimProcs) {
					message+=Lan.g(this,"Procedures attached to claims")+"\r";
				}
				if(hasComm) {
					message+=Lan.g(this,"Commlog entries")+"\r";
				}
				if(hasPayPlans) {
					message+=Lan.g(this,"Payment plans")+"\r";
				}
				if(hasInsPlans) {
					message+=Lan.g(this,"Insurance plans")+"\r";
				}
				if(hasRef) {
					message+=Lan.g(this,"Referrals")+"\r";
				}
				if(hasMeds) {
					message+=Lan.g(this,"Medications")+"\r";
				}
				if(isSuperFamilyHead) {
					message+=Lan.g(this,"Attached Super Family")+"\r";
				}
				if(hasSheets) {
					message+=Lan.g(this,"Sheets")+"\r";
				}
				if(hasRepeat) {
					message+=Lan.g(this,"Repeating Charges")+"\r";
				}
				if(hasCC) {
					message+=Lan.g(this,"Credit Cards")+"\r";
				}
				if(hasRegKey) {
					message+=Lan.g(this,"Registration Keys")+"\r";
				}
				if(hasPerio) {
					message+=Lan.g(this,"Perio Chart")+"\r";
				}
				if(hasClones) {
					message+=Lan.g(this,"Attached Clones")+"\r";
				}
				if(hasDiscount) {
					message+=Lan.g(this,"Discount Plan")+"\r";
				}
				if(hasAllergies) {
					message+=Lan.g(this,"Allergies")+"\r";
				}
				if(hasProblems) {
					message+=Lan.g(this,"Problems")+"\r";
				}
				MessageBox.Show(message);
				return;
			}
			Patient patientOld=_patient.Copy();
			if(_patient.PatNum==_patient.Guarantor) {//if selecting guarantor
				if(_family.ListPats.Length==1) {
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Patient?")) {
						return;
					}
					_patient.PatStatus=PatientStatus.Deleted;
					_patient.ChartNumber="";
					_patient.ClinicNum=0;
					_patient.FeeSched=0;
					Popups.MoveForDeletePat(_patient);
					_patient.SuperFamily=0;
					Patients.Update(_patient,patientOld);
					for(int i=0;i<_listRecalls.Count;i++) {
						if(_listRecalls[i].PatNum==_patient.PatNum) {
							_listRecalls[i].IsDisabled=true;
							_listRecalls[i].DateDue=DateTime.MinValue;
							Recalls.Update(_listRecalls[i]);
						}
					}
					SecurityLogs.MakeLogEntry(EnumPermType.PatientEdit,patientOld.PatNum,"Patient deleted");
					FormOpenDental.S_Contr_PatientSelected(new Patient(),false);
					ModuleSelected(0);
					//does not delete notes or plans, etc.
				}
				else {
					MessageBox.Show(Lan.g(this,"You cannot delete the guarantor if there are other family members. You would have to make a different family member the guarantor first."));
				}
				PatientL.RemoveFromMenu(patientOld.PatNum);//Always remove deleted patients from the dropdown menu.
				return;
			}
			//not selecting guarantor
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Patient?")) {
				return;
			}
			_patient.PatStatus=PatientStatus.Deleted;
			_patient.ChartNumber="";
			_patient.ClinicNum=0;
			_patient.FeeSched=0;
			Popups.MoveForDeletePat(_patient);
			_patient.Guarantor=_patient.PatNum;
			_patient.SuperFamily=0;
			Patients.Update(_patient,patientOld);
			for(int i=0;i<_listRecalls.Count;i++) {
				if(_listRecalls[i].PatNum!=_patient.PatNum) {
					continue;
				}
				_listRecalls[i].IsDisabled=true;
				_listRecalls[i].DateDue=DateTime.MinValue;
				Recalls.Update(_listRecalls[i]);
			}
			SecurityLogs.MakeLogEntry(EnumPermType.PatientEdit,patientOld.PatNum,"Patient deleted");
			ModuleSelected(patientOld.Guarantor);//Sets PatCur to PatOld guarantor.
			FormOpenDental.S_Contr_PatientSelected(_patient,false);//PatCur is now the Guarantor.
			PatientL.RemoveFromMenu(patientOld.PatNum);//Always remove deleted patients from the dropdown menu.
		}

		private void ToolButGuarantor_Click() {
			if(_patient.PatNum==_patient.Guarantor) {
				MessageBox.Show(Lan.g(this,"Patient is already the guarantor.  Please select a different family member."));
				return;
			}
			if(MessageBox.Show(Lan.g(this,"Make the selected patient the guarantor?")
				,"",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
				return;
			}
			if(_patient.SuperFamily==_patient.Guarantor) {//guarantor is also the head of a super family
				Patients.MoveSuperFamily(_patient.SuperFamily,_patient.PatNum);
			}
			Patients.ChangeGuarantorToCur(_family,_patient);
			ModuleSelected(_patient.PatNum);
		}

		private void ToolButMove_Click() {
			Patient patientOld=_patient.Copy();
			//Patient PatCur;
			if(_patient.PatNum==_patient.Guarantor) {//if guarantor selected
				if(_patient.SuperFamily==_patient.Guarantor && _loadData.SuperFamilyMembers.Count>1) {
					MsgBox.Show(this,"You cannot move the head of a super family. If you wish to move the super family head, you must first remove all other super family members.");
					return;
				}
				if(_family.ListPats.Length==1) {//and no other family members
					if(!MovePats(patientOld)) {
						return;
					}
				}
				else {//there are other family members
					for(int i=0;i<_family.ListPats.Count();i++) {
						if(_family.ListPats[i].PatNum==_patient.PatNum) {
							continue;
						}
						List<PatientLink> listPatientLinks=PatientLinks.GetLinks(_family.ListPats[i].PatNum,PatientLinkType.Merge);//If there is another family member, make sure it is merged.  
						if(listPatientLinks.Count==0 || !listPatientLinks.Exists(x => x.PatNumFrom==_family.ListPats[i].PatNum)) {//If it's not merged, user can't move guarantor.
							MessageBox.Show(Lan.g(this,"You cannot move the guarantor.  If you wish to move the guarantor, you must make another family member the guarantor first."));
							return;
						}
					}
					if(!MovePats(patientOld,_family)) {
						return;
					}
				}
				//end guarantor not selected
				ModuleSelected(_patient.PatNum);
				return;
			}
			//guarantor not selected
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Preparing to move family member. Financial notes will not be transferred. Popups will be copied. Proceed to next step?"))
			{
				return;
			}
			if(IsGuarantorTSI()) {
				return;
			}
			switch(MessageBox.Show(Lan.g(this,"Create new family instead of moving to an existing family?"),"",MessageBoxButtons.YesNoCancel)) {
				case DialogResult.Cancel:
					return;
				case DialogResult.Yes://new family (split)
					Popups.CopyForMovingFamilyMember(_patient);//Copy Family Level Popups to new family. 
					//Don't need to copy SuperFamily Popups. Stays in same super family.
					_patient.Guarantor=_patient.PatNum;
					//keep current superfamily
					Patients.Update(_patient,patientOld);
					//if moving a superfamily non-guar family member out as guar of their own family within the sf, and pref is set, add ins to family members if necessary
					if(_patient.SuperFamily>0 && PrefC.GetBool(PrefName.SuperFamNewPatAddIns)) {
						AddSuperGuarPriInsToFam(_patient.Guarantor);
					}
					SecurityLogs.MakeLogEntry(EnumPermType.PatientEdit,_patient.PatNum,"Patient moved to new family.");
					break;
				case DialogResult.No://move to an existing family
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Select the family to move this patient to from the list that will come up next.")) {
						return;
					}
					using(FormPatientSelect formPatientSelect=new FormPatientSelect()) {
						formPatientSelect.IsSelectionModeOnly=true;
						formPatientSelect.ShowDialog();
						if(formPatientSelect.DialogResult!=DialogResult.OK) {
							return;
						}
						Patient patientInNewFam=Patients.GetPat(formPatientSelect.PatNumSelected);
						if(patientInNewFam.Guarantor==_patient.Guarantor) {
							return;// Patient is already a part of the family.
						}
						Popups.CopyForMovingFamilyMember(_patient);//Copy Family Level Popups to new Family. 
						if(_patient.SuperFamily!=patientInNewFam.SuperFamily) {//If they are moving into or out of a superfamily
							if(_patient.SuperFamily!=0) {//If they are currently in a SuperFamily.  Otherwise, no superfamily popups to worry about.
								Popups.CopyForMovingSuperFamily(_patient,patientInNewFam.SuperFamily);
							}
						}
						_patient.Guarantor=patientInNewFam.Guarantor;
						_patient.SuperFamily=patientInNewFam.SuperFamily;//assign to the new superfamily
					}
					Patients.Update(_patient,patientOld);
					SecurityLogs.MakeLogEntry(EnumPermType.PatientEdit,_patient.PatNum,"Patient moved from family of '"+patientOld.Guarantor+"' "
						+"to existing family of '"+_patient.Guarantor+"'");
					break;
			}
			//end guarantor not selected
			ModuleSelected(_patient.PatNum);
		}
		#endregion Methods - Private - GridFamily

		#region Methods - Private - GridRecall
		private void FillGridRecall() {
			//We never show a horizScroll or wrap because there's simply not enough space.  They can double click if they want more info.
			//The window that comes up on the double click could use a bit of help.
			gridRecall.BeginUpdate();
			List<DisplayField> listDisplayFieldsRecall=DisplayFields.GetForCategory(DisplayFieldCategory.FamilyRecallGrid);
			/*int width=0;
			for(int i=0;i<listRecallFields.Count;i++){
				if(i==listRecallFields.Count-1){
					width+=80;//we're counting the last column as 80+
				}
				else{
					width+=listRecallFields[i].ColumnWidth;
				}
			}
			width+=23;//for v scroll*/
			int widthAvail=Width-gridRecall.Left;
			LayoutManager.MoveWidth(gridRecall,widthAvail);
			/*if(LayoutManager.Scale(width)>widthAvail){
				gridRecall.HScrollVisible=true;
			}
			else{
				gridRecall.HScrollVisible=false;
			}*/
			gridRecall.Columns.Clear();
			GridColumn col;
			for(int i=0;i<listDisplayFieldsRecall.Count;i++) {
				if(listDisplayFieldsRecall[i].Description=="") {
					col=new GridColumn(listDisplayFieldsRecall[i].InternalName,listDisplayFieldsRecall[i].ColumnWidth);
					gridRecall.Columns.Add(col);
					continue;
				}
				col=new GridColumn(listDisplayFieldsRecall[i].Description,listDisplayFieldsRecall[i].ColumnWidth);
				gridRecall.Columns.Add(col);
			}
			gridRecall.ListGridRows.Clear();
			if(_patient==null) {
				gridRecall.EndUpdate();
				return;
			}
			//we just want the recall for the current patient
			List<Recall> listRecallsPat=new List<Recall>();
			for(int i=0;i<_listRecalls.Count;i++) {
				if(_listRecalls[i].PatNum==_patient.PatNum) {
					listRecallsPat.Add(_listRecalls[i]);
				}
			}
			GridRow row;
			GridCell cell;
			for(int i=0;i<listRecallsPat.Count;i++) {
				row=new GridRow();
				for(int j=0;j<listDisplayFieldsRecall.Count;j++) {
					switch (listDisplayFieldsRecall[j].InternalName) {
						case "Type":
							string cellStr=RecallTypes.GetDescription(listRecallsPat[i].RecallTypeNum);
							row.Cells.Add(cellStr);
							break;
						case "Due Date":
							if(listRecallsPat[i].DateDue.Year<1880) {
								row.Cells.Add("");
								break;
							}
							cell=new GridCell(listRecallsPat[i].DateDue.ToShortDateString());
							if(listRecallsPat[i].DateDue<DateTime.Today) {
								cell.Bold=YN.Yes;
								cell.ColorText=Color.Firebrick;
							}
							row.Cells.Add(cell);
							break;
						case "Sched Date":
							if(listRecallsPat[i].DateScheduled.Year<1880) {
								row.Cells.Add("");
								break;
							}
							row.Cells.Add(listRecallsPat[i].DateScheduled.ToShortDateString());
							break;
						case "Notes":
							cellStr="";
							if(listRecallsPat[i].TimePatternOverride!="") {
								cellStr+="Time Pattern Override: "+listRecallsPat[i].TimePatternOverride;
							}
							if(listRecallsPat[i].IsDisabled) {
								if(cellStr!="") {
									cellStr+=", ";
								}
								cellStr+=Lan.g(this,"Disabled");
								if(listRecallsPat[i].DatePrevious.Year>1800) {
									cellStr+=Lan.g(this,". Previous: ")+listRecallsPat[i].DatePrevious.ToShortDateString();
									if(listRecallsPat[i].RecallInterval!=new Interval(0,0,0,0)) {
										DateTime dateDue=listRecallsPat[i].DatePrevious+listRecallsPat[i].RecallInterval;
										cellStr+=Lan.g(this,". (Due): ")+dateDue.ToShortDateString();
									}
								}
							}
							if(listRecallsPat[i].DisableUntilDate.Year>1880) {
								if(cellStr!="") {
									cellStr+=", ";
								}
								cellStr+=Lan.g(this,"Disabled until ")+listRecallsPat[i].DisableUntilDate.ToShortDateString();
							}
							if(listRecallsPat[i].DisableUntilBalance>0) {
								if(cellStr!="") {
									cellStr+=", ";
								}
								cellStr+=Lan.g(this,"Disabled until balance ")+listRecallsPat[i].DisableUntilBalance.ToString("c");
							}
							if(listRecallsPat[i].RecallStatus!=0) {
								if(cellStr!="") {
									cellStr+=", ";
								}
								cellStr+=Defs.GetName(DefCat.RecallUnschedStatus,listRecallsPat[i].RecallStatus);
							}
							if(listRecallsPat[i].Note!="") {
								if(cellStr!="") {
									cellStr+=", ";
								}
								cellStr+=listRecallsPat[i].Note;
							}
							row.Cells.Add(cellStr);
							break;
						case "Previous Date":
							if(listRecallsPat[i].DatePrevious.Year>1880) {
								row.Cells.Add(listRecallsPat[i].DatePrevious.ToShortDateString());
								break;
							}
							row.Cells.Add("");
							break;
						case "Interval":
							row.Cells.Add(listRecallsPat[i].RecallInterval.ToString());
							break;
					}
				}
				gridRecall.ListGridRows.Add(row);
			}
			gridRecall.EndUpdate();
		}
		#endregion Methods - Private - GridRecall

		#region Methods - Private - GridSuperFam
		///<summary>Adds the super family guarantor's primary insurance plan to each family member in Fam.  Each family member will be their own
		///subscriber with SubscriberID set to the patient's MedicaidID if one has been entered for the patient.  If a family member does not have a 
		///MedicaidID entered, FormInsPlan will open and prompt the user to enter a SubscriberID.</summary>
		private void AddSuperGuarPriInsToFam(long guarNum) {
			Patient patientSuperFamGuar=Patients.GetPat(_patient.SuperFamily);
			if(patientSuperFamGuar==null) {//should never happen, but just in case
				return;
			}
			List<InsSub> listInsSubsSuper=InsSubs.GetListForSubscriber(patientSuperFamGuar.PatNum);
			if(listInsSubsSuper.Count==0) {//super family guar is not the subscriber for any insplans
				return;
			}
			List<PatPlan> listPatPlansSuper=PatPlans.Refresh(patientSuperFamGuar.PatNum);
			if(listPatPlansSuper.Count==0) {//super family guar doesn't have an active insplan
				return;
			}
			List<InsPlan> listInsPlansSuper=InsPlans.RefreshForSubList(listInsSubsSuper);
			InsSub insSub=InsSubs.GetSub(
				PatPlans.GetInsSubNum(listPatPlansSuper,PatPlans.GetOrdinal(PriSecMed.Primary,listPatPlansSuper,listInsPlansSuper,listInsSubsSuper)),
				listInsSubsSuper);
			if(insSub.InsSubNum==0 //should never happen, an active insplan exists, GetSub should return the inssub for the pri plan, just in case
				|| !MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to add the super family guarantor's primary insurance plan to the patients in this family?"))
			{
				return;
			}
			//super family guarantor has a primary ins plan and the user chose to add it to the patients in this family
			PatPlan patPlanNew;
			InsSub insSub1;
			List<PatPlan> listPatPlansForPat;
			Family family=Patients.GetFamily(guarNum);
			List<InsSub> listInsSubsForFam=InsSubs.RefreshForFam(family);
			List<InsPlan> listInsPlansForFam=InsPlans.RefreshForSubList(listInsSubsForFam);
			bool hasPatPlanAdded=false;
			for(int i=0;i<family.ListPats.Count();i++) {//possibly filter by PatStatus, i.e. .Where(x => x.PatStatus==PatientStatus.Patient)
				listPatPlansForPat=PatPlans.Refresh(family.ListPats[i].PatNum);
				insSub1=listInsSubsForFam.FirstOrDefault(x => x.Subscriber==family.ListPats[i].PatNum && x.PlanNum==insSub.PlanNum);
				if(insSub1!=null) {//InsSub already exists for this Patient and InsPlan
					if(listPatPlansForPat.Any(x => x.InsSubNum==insSub1.InsSubNum)) {//PatPlan exists for this Patient and InsSub, nothing to do
						continue;
					}
				}
				else {//insSub1==null, no InsSub exists for this patient and plan, insert new one
					insSub1=new InsSub();
					insSub1.PlanNum=insSub.PlanNum;
					insSub1.Subscriber=family.ListPats[i].PatNum;
					insSub1.ReleaseInfo=insSub.ReleaseInfo;
					insSub1.AssignBen=insSub.AssignBen;
					//insSubNew.BenefitNotes=sub.BenefitNotes;//not the BenefitNotes, since these could be specific to a patient
					insSub1.SubscriberID=string.IsNullOrWhiteSpace(family.ListPats[i].MedicaidID)?"":family.ListPats[i].MedicaidID;
					//insSubNew.SubscNote=sub.SubscNote;//not the subscriber note, since every patient in super family is their own subscriber to this plan
					insSub1.InsSubNum=InsSubs.Insert(insSub1);
					listInsSubsForFam.Add(insSub1.Copy());
				}
				patPlanNew=new PatPlan();
				patPlanNew.Ordinal=(byte)(listPatPlansForPat.Count+1);//so the ordinal of the first entry will be 1, NOT 0.
				patPlanNew.PatNum=family.ListPats[i].PatNum;
				patPlanNew.InsSubNum=insSub1.InsSubNum;
				patPlanNew.Relationship=Relat.Self;
				patPlanNew.PatPlanNum=PatPlans.Insert(patPlanNew);
				listPatPlansForPat.Add(patPlanNew.Copy());
				if(string.IsNullOrWhiteSpace(insSub1.SubscriberID)) {
					MessageBox.Show(this,Lan.g(this,"Enter the SubscriberID for")+" "+family.ListPats[i].GetNameFL()+".");
					using FormInsPlan formInsPlan=new FormInsPlan(InsPlans.GetPlan(insSub1.PlanNum,listInsPlansForFam),patPlanNew,insSub1);
					formInsPlan.IsNewPlan=false;
					formInsPlan.IsNewPatPlan=true;
					formInsPlan.ShowDialog();//this updates estimates. If cancel, then patplan is deleted. If cancel and planIsNew, then plan and benefits are deleted
					if(formInsPlan.DialogResult!=DialogResult.OK) {
						continue;
					}
				}
				else {
					//compute estimates with new insurance plan
					List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(family.ListPats[i].PatNum);
					List<Procedure> listProcedures=Procedures.Refresh(family.ListPats[i].PatNum);
					List<Benefit> listBenefits=Benefits.Refresh(listPatPlansForPat,listInsSubsForFam);
					Procedures.ComputeEstimatesForAll(family.ListPats[i].PatNum,listClaimProcs,listProcedures,listInsPlansForFam,listPatPlansForPat,listBenefits,family.ListPats[i].Age,listInsSubsForFam);
				}
				hasPatPlanAdded=true;
				if(family.ListPats[i].HasIns!="I") {
					Patient patientOld=family.ListPats[i].Copy();
					family.ListPats[i].HasIns="I";
					Patients.Update(family.ListPats[i],patientOld);
				}
				Appointments.UpdateInsPlansForPat(family.ListPats[i].PatNum);
			}
			if(hasPatPlanAdded) {
				SecurityLogs.MakeLogEntry(EnumPermType.PatPlanCreate,patientSuperFamGuar.PatNum,"Inserted new PatPlans for each family member of the super family guarantor.");
			}
		}

		private void FillGridSuperFam() {
			gridSuperFam.BeginUpdate();
			gridSuperFam.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("gridSuperFam","Name"),280);
			gridSuperFam.Columns.Add(col);
			col=new GridColumn(Lan.g("gridSuperFam","Stmt"),280){ IsWidthDynamic=true };
			gridSuperFam.Columns.Add(col);
			gridSuperFam.ListGridRows.Clear();
			if(_patient==null) {
				gridSuperFam.EndUpdate();
				return;
			}
			GridRow row;
			_listPatientsSuperFamilyGuarantors.Sort(sortPatientListBySuperFamily);
			_listPatientsSuperFamilyMembers.Sort(sortPatientListBySuperFamily);
			List<Patient> listPatientsSuperFamilyMembersNotMerged=_listPatientsSuperFamilyMembers.FindAll(x => !PatientLinks.WasPatientMerged(x.PatNum,_loadData.ListMergeLinks) || x.PatNum==_patient.PatNum);
			string strSuperFam="";
			//Loop through each family within the super family.
			for(int i=0;i<_listPatientsSuperFamilyGuarantors.Count;i++) {
				row=new GridRow();
				//Make a string that displays all the names of the family members.
				//Always start with the guarantor followed by the rest of the family.
				strSuperFam=_listPatientsSuperFamilyGuarantors[i].GetNameLF();
				for(int j=0;j<listPatientsSuperFamilyMembersNotMerged.Count;j++) {
					if(listPatientsSuperFamilyMembersNotMerged[j].Guarantor!=_listPatientsSuperFamilyGuarantors[i].Guarantor) {
						continue;//Not part of this family.
					}
					if(listPatientsSuperFamilyMembersNotMerged[j].PatNum==_listPatientsSuperFamilyGuarantors[i].PatNum) {
						continue;//Guarantor is already in the string.
					}
					strSuperFam+="\r\n   "+StringTools.Truncate(listPatientsSuperFamilyMembersNotMerged[j].GetNameLF(),40,true);
				}
				row.Cells.Add(strSuperFam);
				row.Tag=_listPatientsSuperFamilyGuarantors[i].PatNum;
				if(i==0) {
					row.Cells[0].Bold=YN.Yes;
					row.Cells[0].ColorText=Color.OrangeRed;
				}
				if(_listPatientsSuperFamilyGuarantors[i].HasSuperBilling) {
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}
				gridSuperFam.ListGridRows.Add(row);
			}
			gridSuperFam.EndUpdate();
			for(int i=0;i<gridSuperFam.ListGridRows.Count;i++) {
				if((long)gridSuperFam.ListGridRows[i].Tag==_patient.Guarantor) {
					gridSuperFam.SetSelected(i,true);
					break;
				}
			}
		}

		private int sortPatientListBySuperFamily(Patient patient1,Patient patient2) {
			if(patient1.PatNum==patient2.PatNum) {
				return 0;
			}
			if(patient1.PatNum==patient1.SuperFamily) {//Superheads always go to the top no matter what.
				return -1;
			}
			if(patient2.PatNum==patient2.SuperFamily) {
				return 1;
			}
			switch(_sortStrategySuperFam) {
				case SortStrategy.NameAsc:
					return patient1.GetNameLF().CompareTo(patient2.GetNameLF());
				case SortStrategy.NameDesc:
					return patient2.GetNameLF().CompareTo(patient1.GetNameLF());
				case SortStrategy.PatNumAsc:
					return patient1.PatNum.CompareTo(patient2.PatNum);
				case SortStrategy.PatNumDesc:
					return patient2.PatNum.CompareTo(patient1.PatNum);
				default:
					return patient1.PatNum.CompareTo(patient2.PatNum);//Default behavior
			}
		}

		private void ToolButAddSuper_Click() {
			//At HQ, some resellers don't add clients through the reseller portal.
			//Instead, they contact the conversions department and conversions creates a new account for them and adds them to the superfamily.
			//These accounts are acceptable to add because HQ understands they are not accounts designed to be managed by the Reseller Portal.
			if(_patient.SuperFamily==0) {
				Patients.AssignToSuperfamily(_patient.Guarantor,_patient.Guarantor);
				SecurityLogs.MakeLogEntry(
					EnumPermType.PatientEdit,
					_patient.PatNum,
					Lan.g(this,"Patient added to superfamily. Previous superfamily guarantor PatNum:0, and current superfamily guarantor PatNum:") +_patient.SuperFamily + "."
					
					);
				ModuleSelected(_patient.PatNum);
				return;
			}
			//we must want to add some other family to this superfamily
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.IsSelectionModeOnly=true;
			if(formPatientSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			Patient patientSelected=Patients.GetPat(formPatientSelect.PatNumSelected);
			long superFamilyGuarantorNumPrevious=patientSelected.SuperFamily;
			if(patientSelected.SuperFamily==_patient.SuperFamily) {
				MsgBox.Show(this,"That patient is already part of this superfamily.");
				return;
			}
			List<Patient> listPatientsSuperFam=new List<Patient>();
			if(patientSelected.SuperFamily==patientSelected.Guarantor) {//selected patient's guarantor is the super family head of another super family
				listPatientsSuperFam=Patients.GetBySuperFamily(patientSelected.SuperFamily);
			}
			DialogResult diagResult=DialogResult.None;
			if(listPatientsSuperFam.Any(x => x.Guarantor!=x.SuperFamily)) {//super family consists of more than one family
				//The selected pat's guarantor is the super fam head of another super fam and there are other fams in that super fam.
				//We need to either disband the selected pat's current super fam before moving the selected pat's fam to this super fam or move all super fam
				//members into this super fam (merge the two super fams with this current super fam head) or allow the user to cancel the action.
				string msgTxt=Lans.g(this,"You are about to move the head of another super family.  Would you like to move all members of that super family "
						+"to this super family?")+"\r\n\r\n"
					+Lans.g(this,"Yes - All members of the selected super family will be moved to this super family.")+"\r\n\r\n"
					+Lans.g(this,"No - The selected patient's current super family will be disbanded and only the selected patient's family will be added to "
						+"this super family.")+"\r\n\r\n"
					+Lans.g(this,"Cancel - Do nothing.");
				diagResult=MessageBox.Show(this,msgTxt,"",MessageBoxButtons.YesNoCancel);
			}
			if(diagResult==DialogResult.Cancel) {
				return;//don't need to do ModuleSelected, just return
			}
			if(diagResult==DialogResult.Yes) {
				Patients.MoveSuperFamily(patientSelected.SuperFamily,_patient.SuperFamily);
				if(PrefC.GetBool(PrefName.SuperFamNewPatAddIns)) {
					listPatientsSuperFam.Select(x => x.Guarantor).Distinct().ForEach(x => AddSuperGuarPriInsToFam(x));
				}
				ModuleSelected(_patient.PatNum);
				return;
			}
			if(!diagResult.In(DialogResult.None,DialogResult.No)) {
				ModuleSelected(_patient.PatNum);
				return;
			}
			//None = the fam doesn't belong to another super fam, just move into this super fam
			if(diagResult==DialogResult.No) {
				List<long> listPatientNumsInSuperFam=Patients.GetAllFamilyPatNumsForSuperFam(new List<long>(){superFamilyGuarantorNumPrevious});
				for(int i=0;i<listPatientNumsInSuperFam.Count;i++) { //Create SecurityLog for each member of the superfamily being disbanded.
					if(listPatientNumsInSuperFam[i]==patientSelected.PatNum) { //SecurityLog for the SuperHead moving to another SuperFamily is already handled.
						continue;
					}
					SecurityLogs.MakeLogEntry(
						EnumPermType.PatientEdit,
						listPatientNumsInSuperFam[i],
						Lan.g(this,"Patient removed from superfamily. Previous superfamily guarantor PatNum:")+superFamilyGuarantorNumPrevious+
						Lan.g(this,". Superfamily Disbanded.")
					);
				}
				Patients.DisbandSuperFamily(patientSelected.SuperFamily);//adding to this super family will happen below
			}
			Patients.AssignToSuperfamily(patientSelected.Guarantor,_patient.SuperFamily);
			SecurityLogs.MakeLogEntry(
				EnumPermType.PatientEdit,
				patientSelected.PatNum,
				Lan.g(this,"Patient added to superfamily. Previous superfamily guarantor PatNum:")+superFamilyGuarantorNumPrevious+
				Lan.g(this, " and current superfamily guarantor PatNum:") +_patient.SuperFamily + "."
				);
			if(PrefC.GetBool(PrefName.SuperFamNewPatAddIns)) {
				AddSuperGuarPriInsToFam(patientSelected.Guarantor);
			}
			ModuleSelected(_patient.PatNum);
		}

		private void ToolButDisbandSuper_Click() {
			if(_patient.SuperFamily==0) {
				return;
			}
			Patient patientSuperHead=Patients.GetPat(_patient.SuperFamily);
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Would you like to disband and remove all members in the super family of "+patientSuperHead.GetNameFL()+"?")) {
				return;
			}
			Popups.RemoveForDisbandingSuperFamily(_patient);
			List<long> listPatientNumsInSuperFam=Patients.GetAllFamilyPatNumsForSuperFam(new List<long>(){_patient.SuperFamily});
			for(int i=0;i<listPatientNumsInSuperFam.Count;i++) {
				SecurityLogs.MakeLogEntry(
					EnumPermType.PatientEdit,
					listPatientNumsInSuperFam[i],
					Lan.g(this,"Patient removed from superfamily. Previous superfamily guarantor PatNum:")+_patient.SuperFamily+
					Lan.g(this,". Superfamily Disbanded.")
					);
			}
			Patients.DisbandSuperFamily(patientSuperHead.PatNum);
			ModuleSelected(_patient.PatNum);
		}

		private void ToolButRemoveSuper_Click() {
			long superFamilyGuarantorNumPrevious=_patient.SuperFamily;
			if(_patient.SuperFamily==_patient.Guarantor) {
				MsgBox.Show(this,"You cannot delete the head of a super family.");
				return;
			}
			if(_patient.SuperFamily==0) {
				return;
			}
			for(int i=0;i<_family.ListPats.Length;i++) {//remove whole family
				Patient patientTemp=_family.ListPats[i].Copy();
				Popups.CopyForMovingSuperFamily(patientTemp,0);
				patientTemp.SuperFamily=0;
				Patients.Update(patientTemp,_family.ListPats[i]);
				SecurityLogs.MakeLogEntry(
					EnumPermType.PatientEdit,
					patientTemp.PatNum,
					Lan.g(this,"Patient removed from superfamily. Previous superfamily guarantor PatNum:")+superFamilyGuarantorNumPrevious+"."
					);
			}
			ModuleSelected(_patient.PatNum);
		}
		#endregion Methods - Private - GridSuperFam

		#region Methods - Private - GridIns
		private void FillInsData() {
			if(_patient!=null && _discountPlanSub!=null) {
				gridIns.BeginUpdate();
				gridIns.Title=Lan.g(this,"Discount Plan");
				gridIns.Columns.Clear();
				gridIns.ListGridRows.Clear();
				gridIns.Columns.Add(new GridColumn("",170));
				gridIns.Columns.Add(new GridColumn(Lan.g(this,"Discount Plan"),170));
				DiscountPlan discountPlan;
				if(_loadData.DiscountPlan==null || _loadData.DiscountPlan.DiscountPlanNum!=_discountPlanSub.DiscountPlanNum) {
					discountPlan=DiscountPlans.GetPlan(_discountPlanSub.DiscountPlanNum);
				}
				else {
					discountPlan=_loadData.DiscountPlan;
				}
				Def defAdjType=Defs.GetDef(DefCat.AdjTypes,discountPlan.DefNum);
				GridRow rowDiscount=new GridRow();
				rowDiscount.Cells.Add(Lan.g("TableDiscountPlans","Description"));
				rowDiscount.Cells.Add(discountPlan.Description);
				rowDiscount.ColorBackG=Defs.GetFirstForCategory(DefCat.MiscColors).ItemColor;
				gridIns.ListGridRows.Add(rowDiscount);
				rowDiscount=new GridRow();
				rowDiscount.Cells.Add(Lan.g("TableDiscountPlans","Adjustment Type"));
				rowDiscount.Cells.Add(defAdjType.ItemName);
				gridIns.ListGridRows.Add(rowDiscount);
				rowDiscount=new GridRow();
				rowDiscount.Cells.Add(Lan.g("TableDiscountPlans","Fee Schedule"));
				rowDiscount.Cells.Add(FeeScheds.GetDescription(discountPlan.FeeSchedNum));
				gridIns.ListGridRows.Add(rowDiscount);
				rowDiscount=new GridRow();
				rowDiscount.Cells.Add(Lan.g("TableDiscountPlans","Date Effective"));
				if(_discountPlanSub.DateEffective.Year < 1880) {
					rowDiscount.Cells.Add("");
				}
				else {
					rowDiscount.Cells.Add(_discountPlanSub.DateEffective.ToShortDateString());
				}
				string discountPlanAnnualMax=discountPlan.AnnualMax.ToString();
				if(discountPlan.AnnualMax==-1) {
					discountPlanAnnualMax="";
				}
				gridIns.ListGridRows.Add(new GridRow(Lan.g("TableDiscountPlans","Annual Max"),discountPlanAnnualMax));
				if(discountPlan.ExamFreqLimit>=0) {
					gridIns.ListGridRows.Add(new GridRow(Lan.g("TableDiscountPlans","Exam Frequency"),discountPlan.ExamFreqLimit.ToString()));
				}
				if(discountPlan.ProphyFreqLimit>=0) {
					gridIns.ListGridRows.Add(new GridRow(Lan.g("TableDiscountPlans","Prophy Frequency"),discountPlan.ProphyFreqLimit.ToString()));
				}
				if(discountPlan.FluorideFreqLimit>=0) {
					gridIns.ListGridRows.Add(new GridRow(Lan.g("TableDiscountPlans","Fluoride Frequency"),discountPlan.FluorideFreqLimit.ToString()));
				}
				if(discountPlan.PerioFreqLimit>=0) {
					gridIns.ListGridRows.Add(new GridRow(Lan.g("TableDiscountPlans","Perio Frequency"),discountPlan.PerioFreqLimit.ToString()));
				}
				if(discountPlan.LimitedExamFreqLimit>=0) {
					gridIns.ListGridRows.Add(new GridRow(Lan.g("TableDiscountPlans","Limited Frequency"),discountPlan.LimitedExamFreqLimit.ToString()));
				}
				if(discountPlan.XrayFreqLimit>=0) {
					gridIns.ListGridRows.Add(new GridRow(Lan.g("TableDiscountPlans","X-Ray Frequency"),discountPlan.XrayFreqLimit.ToString()));
				}
				if(discountPlan.PAFreqLimit>=0) {
					gridIns.ListGridRows.Add(new GridRow(Lan.g("TableDiscountPlans","PA Frequency"),discountPlan.PAFreqLimit.ToString()));
				}
				gridIns.ListGridRows.Add(rowDiscount);
				rowDiscount=new GridRow();
				rowDiscount.Cells.Add(Lan.g("TableDiscountPlans","Date Term"));
				if(_discountPlanSub.DateTerm.Year < 1880) {
					rowDiscount.Cells.Add("");
				}
				else {
					rowDiscount.Cells.Add(_discountPlanSub.DateTerm.ToShortDateString());
					//Here is where we would add colors if we wanted an indicator
					if(_discountPlanSub.DateTerm<DateTime.Today) {
						rowDiscount.Bold=true;
						rowDiscount.ColorBackG=Color.LightSalmon;
					}
				}
				gridIns.ListGridRows.Add(rowDiscount);
				//add in grid row for plan note for the disount plan
				rowDiscount=new GridRow();
				rowDiscount.Cells.Add(Lan.g("TableDiscountPlans","Plan Note"));
				GridCell cellNote = new GridCell();
				cellNote.Text=StringTools.Truncate(discountPlan.PlanNote,1000,hasElipsis:true);
				cellNote.Bold=YN.Yes;
				cellNote.ColorText=Color.Red;
				rowDiscount.Cells.Add(cellNote);
				gridIns.ListGridRows.Add(rowDiscount);
				//add grid row for subscriber note for the discount plan's subscriber
				rowDiscount=new GridRow();
				rowDiscount.Cells.Add(Lan.g("TableDiscountPlans","Subscriber Note"));
				cellNote = new GridCell();
				cellNote.Text=StringTools.Truncate(_discountPlanSub.SubNote,1000,hasElipsis:true);
				cellNote.Bold=YN.Yes;
				cellNote.ColorText=Color.Red;
				rowDiscount.Cells.Add(cellNote);
				gridIns.ListGridRows.Add(rowDiscount);
				gridIns.EndUpdate();
				return;
			}
			else {
				gridIns.Title=Lan.g(this,"Insurance Plans");
			}
			if(_listPatPlans.Count==0){
				gridIns.BeginUpdate();
				gridIns.Columns.Clear();
				gridIns.ListGridRows.Clear();
				gridIns.EndUpdate();
				return;
			}
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.MiscColors);
			List<InsSub> listInsSubs=new List<InsSub>();//prevents repeated calls to db.
			List<InsPlan> listInsPlans=new List<InsPlan>();
			InsSub insSub;
			for(int i=0;i<_listPatPlans.Count;i++) {
				insSub=InsSubs.GetSub(_listPatPlans[i].InsSubNum,_listInsSubs);
				listInsSubs.Add(insSub);
				listInsPlans.Add(InsPlans.GetPlan(insSub.PlanNum,_listInsPlans));
			}
			gridIns.BeginUpdate();
			gridIns.Columns.Clear();
			gridIns.ListGridRows.Clear();
			GridColumn col;
			col=new GridColumn("",150);
			gridIns.Columns.Add(col);
			int dentalOrdinal=1;
			for(int i=0;i<_listPatPlans.Count;i++) {
				if(listInsPlans[i].IsMedical) {
					col=new GridColumn(Lan.g("TableCoverage","Medical"),170);
					gridIns.Columns.Add(col);
					continue;
				}
				//dental
				if(dentalOrdinal==1) {
					col=new GridColumn(Lan.g("TableCoverage","Primary"),170);
					gridIns.Columns.Add(col);
				}
				else if(dentalOrdinal==2) {
					col=new GridColumn(Lan.g("TableCoverage","Secondary"),170);
					gridIns.Columns.Add(col);
				}
				else {
					col=new GridColumn(Lan.g("TableCoverage","Other"),170);
					gridIns.Columns.Add(col);
				}
				dentalOrdinal++;
			}
			OpenDental.UI.GridRow row=new GridRow();
			//subscriber
			row.Cells.Add(Lan.g("TableCoverage","Subscriber"));
			for(int i=0;i<_listPatPlans.Count;i++) {
				row.Cells.Add(_family.GetNameInFamFL(listInsSubs[i].Subscriber));
			}
			row.ColorBackG=listDefs[(int)DefCatMiscColors.FamilyModuleCoverage].ItemColor;
			gridIns.ListGridRows.Add(row);
			//subscriber ID
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Subscriber ID"));
			for(int i=0;i<_listPatPlans.Count;i++) {
				row.Cells.Add(listInsSubs[i].SubscriberID);
			}
			row.ColorBackG=listDefs[(int)DefCatMiscColors.FamilyModuleCoverage].ItemColor;
			gridIns.ListGridRows.Add(row);
			//relationship
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Rel'ship to Sub"));
			for(int i=0;i<_listPatPlans.Count;i++){
				row.Cells.Add(Lan.g("enumRelat",_listPatPlans[i].Relationship.ToString()));
			}
			row.ColorBackG=listDefs[(int)DefCatMiscColors.FamilyModuleCoverage].ItemColor;
			gridIns.ListGridRows.Add(row);
			//patient ID
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Patient ID"));
			for(int i=0;i<_listPatPlans.Count;i++) {
				row.Cells.Add(_listPatPlans[i].PatID);
			}
			row.ColorBackG=listDefs[(int)DefCatMiscColors.FamilyModuleCoverage].ItemColor;
			gridIns.ListGridRows.Add(row);
			//pending
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Pending"));
			for(int i=0;i<_listPatPlans.Count;i++) {
				if(_listPatPlans[i].IsPending){
					row.Cells.Add("X");
					continue;
				}
				row.Cells.Add("");
			}
			row.ColorBackG=listDefs[(int)DefCatMiscColors.FamilyModuleCoverage].ItemColor;
			row.ColorLborder=Color.Black;
			gridIns.ListGridRows.Add(row);
			//employer
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Employer"));
			for(int i=0;i<_listPatPlans.Count;i++) {
				row.Cells.Add(Employers.GetName(listInsPlans[i].EmployerNum));
			}
			gridIns.ListGridRows.Add(row);
			//carrier
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Carrier"));
			for(int i=0;i<_listPatPlans.Count;i++) {
				row.Cells.Add(InsPlans.GetCarrierName(listInsPlans[i].PlanNum,listInsPlans));
			}
			gridIns.ListGridRows.Add(row);
			//group name
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Group Name"));
			for(int i=0;i<_listPatPlans.Count;i++) {
				row.Cells.Add(listInsPlans[i].GroupName);
			}
			gridIns.ListGridRows.Add(row);
			//group number
			row=new GridRow();
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				row.Cells.Add(Lan.g("TableCoverage","Plan Number"));
			}
			else {
				row.Cells.Add(Lan.g("TableCoverage","Group Number"));
			}
			for(int i=0;i<_listPatPlans.Count;i++) {
				row.Cells.Add(listInsPlans[i].GroupNum);
			}
			gridIns.ListGridRows.Add(row);
			//plan type
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Type"));
			for(int i=0;i<listInsPlans.Count;i++) {
				switch(listInsPlans[i].PlanType){
					default://malfunction
						row.Cells.Add("");
						break;
					case "":
						row.Cells.Add(Lan.g(this,"Category Percentage"));
						break;
					case "p":
						FeeSched feeSchedCopay=FeeScheds.GetFirstOrDefault(x => x.FeeSchedNum==listInsPlans[i].CopayFeeSched);
						if(feeSchedCopay!=null && feeSchedCopay.FeeSchedType==FeeScheduleType.FixedBenefit) {
							row.Cells.Add(Lan.g(this,"PPO Fixed Benefit"));
							break;
						}
						row.Cells.Add(Lan.g(this,"PPO Percentage"));
						break;
					case "f":
						row.Cells.Add(Lan.g(this,"Medicaid or Flat Co-pay"));
						break;
					case "c":
						row.Cells.Add(Lan.g(this,"Capitation"));
						break;
				}
			}
			gridIns.ListGridRows.Add(row);
			//fee schedule
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Fee Schedule"));
			for(int i=0;i<listInsPlans.Count;i++) {
				row.Cells.Add(FeeScheds.GetDescription(listInsPlans[i].FeeSched));
			}
			row.ColorLborder=Color.Black;
			gridIns.ListGridRows.Add(row);
			//Calendar vs service year------------------------------------------------------------------------------------
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Benefit Period"));
			for(int i=0;i<listInsPlans.Count;i++) {
				if(listInsPlans[i].MonthRenew==0) {
					row.Cells.Add(Lan.g("TableCoverage","Calendar Year"));
					continue;
				}
				DateTime dateService=new DateTime(2000,listInsPlans[i].MonthRenew,1);
				row.Cells.Add(Lan.g("TableCoverage","Service year begins:")+" "+dateService.ToString("MMMM"));
			}
			gridIns.ListGridRows.Add(row);
			//Benefits-----------------------------------------------------------------------------------------------------
			Benefit[,] benefitMatrix=Benefits.GetDisplayMatrix(_loadData.ListBenefits,_listPatPlans,_listInsSubs);
			string strDesc;
			string strVal;
			ProcedureCode procedureCode=null;
			for(int y=0;y<benefitMatrix.GetLength(1);y++) {//rows
				bool isAgeLimitation=false;
				row=new GridRow();
				strDesc="";
				//some of the columns might be null, but at least one will not be.  Find it.
				for(int x=0;x<benefitMatrix.GetLength(0);x++) {//columns
					if(benefitMatrix[x,y]==null){
						continue;
					}
					//create a description for the benefit
					if(benefitMatrix[x,y].PatPlanNum!=0) {
						strDesc+=Lan.g(this,"(pat)")+" ";
					}
					if(benefitMatrix[x,y].CoverageLevel==BenefitCoverageLevel.Family) {
						strDesc+=Lan.g(this,"Fam")+" ";
					}
					procedureCode=ProcedureCodes.GetProcCode(benefitMatrix[x,y].CodeNum);
					if(benefitMatrix[x,y].BenefitType==InsBenefitType.CoInsurance && benefitMatrix[x,y].Percent!=-1) {
						if(benefitMatrix[x,y].CodeNum==0) {
							strDesc+=CovCats.GetDesc(benefitMatrix[x,y].CovCatNum)+" % ";
						}
						else {
							strDesc+=procedureCode.ProcCode+"-"+procedureCode.AbbrDesc+" % ";
						}
					}
					else if(benefitMatrix[x,y].BenefitType==InsBenefitType.Deductible) {
						strDesc+=Lan.g(this,"Deductible")+" "+CovCats.GetDesc(benefitMatrix[x,y].CovCatNum)+" ";
					}
					else if(benefitMatrix[x,y].BenefitType==InsBenefitType.Limitations
						&& benefitMatrix[x,y].QuantityQualifier==BenefitQuantity.None
						&& (benefitMatrix[x,y].TimePeriod==BenefitTimePeriod.ServiceYear
						|| benefitMatrix[x,y].TimePeriod==BenefitTimePeriod.CalendarYear))
					{
						strDesc+=Lan.g(this,"Annual Max")+" ";
					}
					else if(benefitMatrix[x,y].BenefitType==InsBenefitType.Limitations
						&& CovCats.GetForEbenCat(EbenefitCategory.Orthodontics)!=null
						&& benefitMatrix[x,y].CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum
						&& benefitMatrix[x,y].QuantityQualifier==BenefitQuantity.None
						&& benefitMatrix[x,y].TimePeriod==BenefitTimePeriod.Lifetime)
					{
						strDesc+=Lan.g(this,"Ortho Max")+" ";
					}
					else if(Benefits.IsFluorideAgeLimit(benefitMatrix[x,y])) {
						strDesc+=Lan.g(this,"Fluoride age limit")+" ";
						isAgeLimitation=true;
					}
					else if(Benefits.IsSealantAgeLimit(benefitMatrix[x,y])) {
						strDesc+=Lan.g(this,"Sealant age limit")+" ";
						isAgeLimitation=true;
					}
					else if(benefitMatrix[x,y].CodeGroupNum!=0) {
						strDesc+=CodeGroups.GetGroupName(benefitMatrix[x,y].CodeGroupNum,isShort:true)+" "+Lan.g(this,"frequency")+" ";
					}
					else if(benefitMatrix[x,y].CodeNum==0 && procedureCode.AbbrDesc!=null) {//e.g. flo
						strDesc+=procedureCode.AbbrDesc+" ";
					}
					else {
						strDesc+=Lan.g("enumInsBenefitType",benefitMatrix[x,y].BenefitType.ToString())+" ";
					}
					row.Cells.Add(strDesc);
					break;
				}
				//remember that matrix does not include the description column
				for(int x=0;x<benefitMatrix.GetLength(0);x++) {//columns
					strVal="";
					//this matrix cell might be null
					if(benefitMatrix[x,y]==null) {
						row.Cells.Add("");
						continue;
					}
					if(benefitMatrix[x,y].Percent != -1) {
						strVal+=benefitMatrix[x,y].Percent.ToString()+"% ";
					}
					if(benefitMatrix[x,y].MonetaryAmt != -1) {
						strVal+=benefitMatrix[x,y].MonetaryAmt.ToString("c0")+" ";
					}
					/*
					if(benMatrix[x,y].BenefitType==InsBenefitType.CoInsurance) {
						val+=benMatrix[x,y].Percent.ToString()+" ";
					}
					else if(benMatrix[x,y].BenefitType==InsBenefitType.Deductible
						&& benMatrix[x,y].MonetaryAmt==0)
					{//deductible 0
						val+=benMatrix[x,y].MonetaryAmt.ToString("c0")+" ";
					}
					else if(benMatrix[x,y].BenefitType==InsBenefitType.Limitations
						&& benMatrix[x,y].QuantityQualifier==BenefitQuantity.None
						&& (benMatrix[x,y].TimePeriod==BenefitTimePeriod.ServiceYear
						|| benMatrix[x,y].TimePeriod==BenefitTimePeriod.CalendarYear)
						&& benMatrix[x,y].MonetaryAmt==0)
					{//annual max 0
						val+=benMatrix[x,y].MonetaryAmt.ToString("c0")+" ";
					}*/
					if(benefitMatrix[x,y].BenefitType.In(InsBenefitType.Exclusions,InsBenefitType.Limitations) && !isAgeLimitation) {
						if(benefitMatrix[x,y].CodeNum != 0) {
							procedureCode=ProcedureCodes.GetProcCode(benefitMatrix[x,y].CodeNum);
							strVal+=procedureCode.ProcCode+"-"+procedureCode.AbbrDesc+" ";
						}
						else if(benefitMatrix[x,y].CovCatNum != 0) {
							strVal+=CovCats.GetDesc(benefitMatrix[x,y].CovCatNum)+" ";
						}
					}
					if(benefitMatrix[x,y].QuantityQualifier==BenefitQuantity.NumberOfServices) {//eg 2 times per CalendarYear
						if(benefitMatrix[x,y].TimePeriod==BenefitTimePeriod.NumberInLast12Months) {
							strVal+=benefitMatrix[x,y].Quantity.ToString()+" "+Lan.g(this,"times in the last 12 months")+" ";
						}
						else {
							strVal+=benefitMatrix[x,y].Quantity.ToString()+" "+Lan.g(this,"times per")+" "
								+Lan.g("enumBenefitQuantity",benefitMatrix[x,y].TimePeriod.ToString())+" ";
						}
					}
					else if(benefitMatrix[x,y].QuantityQualifier==BenefitQuantity.Months) {//eg Every 2 months
						strVal+=Lan.g(this,"Every ")+benefitMatrix[x,y].Quantity.ToString()+" month";
						if(benefitMatrix[x,y].Quantity>1) {
							strVal+="s";
						}
					}
					else if(benefitMatrix[x,y].QuantityQualifier==BenefitQuantity.Years) {//eg Every 2 years
						strVal+="Every "+benefitMatrix[x,y].Quantity.ToString()+" year";
						if(benefitMatrix[x,y].Quantity>1) {
							strVal+="s";
						}
					}
					else {
						if(benefitMatrix[x,y].QuantityQualifier!=BenefitQuantity.None && !isAgeLimitation) {//e.g. flo
							strVal+=Lan.g("enumBenefitQuantity",benefitMatrix[x,y].QuantityQualifier.ToString())+" ";
						}
						if(benefitMatrix[x,y].Quantity!=0) {
							strVal+=benefitMatrix[x,y].Quantity.ToString()+" ";
						}
						if(isAgeLimitation) {
							strVal+=Lan.g(this,"years old");
						}
					}
					if(benefitMatrix[x,y].BenefitType==InsBenefitType.WaitingPeriod 
						&& benefitMatrix[x,y].QuantityQualifier.In(BenefitQuantity.Months,BenefitQuantity.Years))
					{
						strVal=CovCats.GetDesc(benefitMatrix[x,y].CovCatNum)+" "+Lan.g(this,"Wait ")+benefitMatrix[x,y].Quantity.ToString();
						if(benefitMatrix[x,y].QuantityQualifier==BenefitQuantity.Months) {//eg Every 2 months
							strVal+=Lan.g(this," Month"+(benefitMatrix[x,y].Quantity>1?"s":""));
						}
						else {//eg Every 2 years
							strVal+=Lan.g(this," Year"+(benefitMatrix[x,y].Quantity>1?"s":""));
						}
					}
					//if(benMatrix[x,y].MonetaryAmt!=0){
					//	val+=benMatrix[x,y].MonetaryAmt.ToString("c0")+" ";
					//}
					//if(val==""){
					//	val="val";
					//}
					row.Cells.Add(strVal);
				}
				gridIns.ListGridRows.Add(row);
			}
			//Plan note
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Ins Plan Note"));
			GridCell cell;
			for(int i=0;i<_listPatPlans.Count;i++) {
				cell=new GridCell();
				cell.Text=listInsPlans[i].PlanNote;
				cell.ColorText=Color.Red;
				cell.Bold=YN.Yes;
				row.Cells.Add(cell);
			}
			gridIns.ListGridRows.Add(row);
			//Subscriber Note
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Subscriber Note"));
			for(int i=0;i<_listPatPlans.Count;i++) {
				cell=new GridCell();
				cell.Text=listInsSubs[i].SubscNote;
				cell.ColorText=Color.Red;
				cell.Bold=YN.Yes;
				row.Cells.Add(cell);
			}
			row.ColorLborder=Color.Black;
			gridIns.ListGridRows.Add(row);
			//InsHist
			List<Procedure> listProceduresEoAndC=Procedures.GetProcsByStatusForPat(_patient.PatNum,new [] { ProcStat.EO,ProcStat.C }); //all for the patient, all plans
			List<long> listProcNums=listProceduresEoAndC.Select(x=>x.ProcNum).ToList();
			List<ClaimProc> listClaimProcsForEoAndCProcs=ClaimProcs.GetForProcs(listProcNums);//all for the patient, all plans
			List<PrefName> listPrefNames=Prefs.GetInsHistPrefNames();
			for(int i=0;i<listPrefNames.Count();i++) {
				row=new GridRow();
				row.Cells.Add(Lan.g("TableCoverage",listPrefNames[i].GetDescription()));
				for(int j=0;j<_listPatPlans.Count();j++) {
					List<ClaimProc> listClaimProcsForPlan=listClaimProcsForEoAndCProcs.FindAll(x=>x.InsSubNum==_listPatPlans[j].InsSubNum && x.Status.In(ClaimProcStatus.InsHist,ClaimProcStatus.Received));
					List<Procedure> listProceduresForPlan=listProceduresEoAndC.FindAll(x=>listClaimProcsForPlan.Any(y=>y.ProcNum==x.ProcNum));
					Procedure procedure=Procedures.GetMostRecentInsHistProc(listProceduresForPlan,ProcedureCodes.GetCodeNumsForInsHistPref(listPrefNames[i]),listPrefNames[i]);
					if(procedure==null) {
						row.Cells.Add(new GridCell(Lan.g("TableCoverage","No History")));
						continue;
					}
					row.Cells.Add(new GridCell(procedure.ProcDate.ToShortDateString()));
				}
				row.Tag=listPrefNames[i].ToString();//Tag with prefname
				gridIns.ListGridRows.Add(row);
			}
			gridIns.EndUpdate();
		}

		private void ToolButDiscount_Click() {
			if(_listPatPlans.Count>0) {
				MsgBox.Show(this,"Cannot add discount plan when patient has insurance.");
				return;
			}
			if(_discountPlanSub==null) {//Patient does not have a discount plan.
				//Let the user pick which discount plan the patient should subscribe to.
				using FormDiscountPlans formDiscountPlans=new FormDiscountPlans();
				formDiscountPlans.IsSelectionMode=true;
				if(formDiscountPlans.ShowDialog()!=DialogResult.OK) {
					return;
				}
				//Give the user an opportunity to edit the subscription.
				using FormDiscountPlanSubEdit formDiscountPlanSubEdit=new FormDiscountPlanSubEdit();
				formDiscountPlanSubEdit.DiscountPlanCur=formDiscountPlans.DiscountPlanSelected;
				formDiscountPlanSubEdit.PatNum=_patient.PatNum;
				if(formDiscountPlanSubEdit.ShowDialog()!=DialogResult.OK || formDiscountPlanSubEdit.DiscountPlanSubCur==null) {
					return;//User either clicked Cancel or Drop, nothing to do.
				}
				_discountPlanSub=formDiscountPlanSubEdit.DiscountPlanSubCur;
			}
			else {
				using FormDiscountPlanSubEdit formDiscountPlanSubEdit=new FormDiscountPlanSubEdit();
				formDiscountPlanSubEdit.DiscountPlanSubCur=_discountPlanSub;
				if(formDiscountPlanSubEdit.ShowDialog()!=DialogResult.OK) {
					return;
				}
				_discountPlanSub=formDiscountPlanSubEdit.DiscountPlanSubCur;
			}
			//Update all active and inactive treatment plans regardless if the user added or dropped a discount plan.
			TreatPlans.UpdateTreatmentPlanType(_patient);
			FillInsData();
		}

		private void ToolButIns_Click() {
			if(_discountPlanSub!=null) {
				MsgBox.Show(this,"Cannot add insurance if patient has a discount plan.");
				return;
			}
			DialogResult result=MessageBox.Show(Lan.g(this,"Is this patient the subscriber?"),"",MessageBoxButtons.YesNoCancel);
			if(result==DialogResult.Cancel) {
				return;
			}
			//Pick a subscriber------------------------------------------------------------------------------------------------
			Patient patientSubscriber;
			if(result==DialogResult.Yes) {//current patient is subscriber
				patientSubscriber=_patient.Copy();
			}
			else {//patient is not subscriber
				//show list of patients in this family
				using FormSubscriberSelect formSubsciberSelect=new FormSubscriberSelect(_family);
				formSubsciberSelect.ShowDialog();
				if(formSubsciberSelect.DialogResult==DialogResult.Cancel) {
					return;
				}
				patientSubscriber=Patients.GetPat(formSubsciberSelect.PatNumSelected);
			}
			//Subscriber has been chosen. Now, pick a plan-------------------------------------------------------------------
			InsPlan insPlan=null;
			InsSub insSub=null;
			bool isNewPlan=false;
			List<InsSub> listInsSubs=InsSubs.GetListForSubscriber(patientSubscriber.PatNum);
			if(listInsSubs.Count==0) {
				isNewPlan=true;
			}
			else {
				FrmInsSelectSubscr frmInsSelectSubscr=new FrmInsSelectSubscr(patientSubscriber.PatNum,_patient.PatNum);
				frmInsSelectSubscr.ShowDialog();
				if(!frmInsSelectSubscr.IsDialogOK) {
					return;
				}
				if(frmInsSelectSubscr.SelectedInsSubNum==0) {//'New' option selected.
					isNewPlan=true;
				}
				else {
					insSub=InsSubs.GetSub(frmInsSelectSubscr.SelectedInsSubNum,listInsSubs);
					insPlan=InsPlans.GetPlan(insSub.PlanNum,new List<InsPlan>());
				}
			}
			//New plan was selected instead of an existing plan.  Create the plan--------------------------------------------
			if(isNewPlan) {
				insPlan=new InsPlan();
				insPlan.EmployerNum=patientSubscriber.EmployerNum;
				insPlan.PlanType="";
				InsPlans.Insert(insPlan);
				insSub=new InsSub();
				insSub.PlanNum=insPlan.PlanNum;
				insSub.Subscriber=patientSubscriber.PatNum;
				insSub.SubscriberID=patientSubscriber.MedicaidID;
				insSub.ReleaseInfo=true;
				insSub.AssignBen=PrefC.GetBool(PrefName.InsDefaultAssignBen);
				InsSubs.Insert(insSub);
				Benefit benefit;
				List<CovCat> listCovCats=CovCats.GetWhere(x => x.DefaultPercent!=-1,true);
				for(int i=0;i<listCovCats.Count();i++) {
					benefit=new Benefit();
					benefit.BenefitType=InsBenefitType.CoInsurance;
					benefit.CovCatNum=listCovCats[i].CovCatNum;
					benefit.PlanNum=insPlan.PlanNum;
					benefit.Percent=listCovCats[i].DefaultPercent;
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
					benefit.CodeNum=0;
					Benefits.Insert(benefit);
				}
				//Zero deductible diagnostic
				if(CovCats.GetForEbenCat(EbenefitCategory.Diagnostic)!=null) {
					benefit=new Benefit();
					benefit.CodeNum=0;
					benefit.BenefitType=InsBenefitType.Deductible;
					benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum;
					benefit.PlanNum=insPlan.PlanNum;
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
					benefit.MonetaryAmt=0;
					benefit.Percent=-1;
					benefit.CoverageLevel=BenefitCoverageLevel.Individual;
					Benefits.Insert(benefit);
				}
				//Zero deductible preventive
				if(CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive)!=null) {
					benefit=new Benefit();
					benefit.CodeNum=0;
					benefit.BenefitType=InsBenefitType.Deductible;
					benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
					benefit.PlanNum=insPlan.PlanNum;
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
					benefit.MonetaryAmt=0;
					benefit.Percent=-1;
					benefit.CoverageLevel=BenefitCoverageLevel.Individual;
					Benefits.Insert(benefit);
				}
			}
			//Then attach plan------------------------------------------------------------------------------------------------
			PatPlan patplan=new PatPlan();
			patplan.Ordinal=(byte)(_listPatPlans.Count+1);//so the ordinal of the first entry will be 1, NOT 0.
			patplan.PatNum=_patient.PatNum;
			patplan.InsSubNum=insSub.InsSubNum;
			patplan.Relationship=Relat.Self;
			PatPlans.Insert(patplan);
			//Then, display insPlanEdit to user-------------------------------------------------------------------------------
			using FormInsPlan formInsPlan=new FormInsPlan(insPlan,patplan,insSub);
			formInsPlan.IsNewPlan=isNewPlan;
			formInsPlan.IsNewPatPlan=true;
			if(formInsPlan.ShowDialog()!=DialogResult.Cancel) {
				SecurityLogs.MakeLogEntry(EnumPermType.PatPlanCreate,_patient.PatNum,"Inserted new PatPlan for patient. InsPlanNum: "+formInsPlan.GetPlanCurNum());
				//Update users treatment plans to tie in to insurance
				TreatPlans.UpdateTreatmentPlanType(_patient);
			}//this updates estimates also.
			//if cancel, then patplan is deleted from within that dialog.
			//if cancel, and planIsNew, then plan and benefits are also deleted.
			ModuleSelected(_patient.PatNum);
		}
		#endregion Methods - Private - GridIns

		#region Methods - Private - Patient Clones
		private void FillGridPatientClones() {
			gridPatientClones.BeginUpdate();
			gridPatientClones.Columns.Clear();
			gridPatientClones.Columns.Add(new GridColumn(Lan.g(gridPatientClones.TranslationName,"Name"),150));
			if(PrefC.HasClinicsEnabled) {
				gridPatientClones.Columns.Add(new GridColumn(Lan.g(gridPatientClones.TranslationName,"Clinic"),80));
			}
			gridPatientClones.Columns.Add(new GridColumn(Lan.g(gridPatientClones.TranslationName,"Specialty"),150){ IsWidthDynamic=true });
			gridPatientClones.ListGridRows.Clear();
			if(_patient==null) {
				gridPatientClones.EndUpdate();
				return;
			}
			int selectedIndex=-1;
			GridRow row;
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ClinicSpecialty);
			for(int i=0;i<_loadData.ListPatientsClones.Count;i++) {
				//Never add deleted patients to the grid.  Deleted patients should not be selectable.
				if(_loadData.ListPatientsClones[i].PatStatus==PatientStatus.Deleted) {
					continue;
				}
				row=new GridRow();
				row.Cells.Add(_loadData.ListPatientsClones[i].GetNameLF());
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(_loadData.ListPatientsClones[i].ClinicNum));
				}
				DefLink defLink=_loadData.ListDefLinksSpecialty.Find(x=>x.FKey==_loadData.ListPatientsClones[i].PatNum);
				//Check for null because an office could have just turned on clinics and a specialty would not have been required prior.
				if(defLink==null) {
					row.Cells.Add("");
				}
				else {
					Def defSpecialty=Defs.GetDef(DefCat.ClinicSpecialty,defLink.DefNum,listDefs);//DefNum will be valid
					row.Cells.Add(defSpecialty.ItemName);
				}
				row.Tag=_loadData.ListPatientsClones[i];
				//If we are about to add the clone that is currently selected, save the index of said patient so that we can select them after the update.
				if(_patient!=null && _loadData.ListPatientsClones[i].PatNum==_patient.PatNum) {
					selectedIndex=gridPatientClones.ListGridRows.Count;
				}
				gridPatientClones.ListGridRows.Add(row);
			}
			//The first entry will always be the original or master patient which we want to stand out a little bit much like the Super Family grid.
			if(gridPatientClones.ListGridRows.Count>0) {
				gridPatientClones.ListGridRows[0].Cells[0].Bold=YN.Yes;
				gridPatientClones.ListGridRows[0].Cells[0].ColorText=Color.OrangeRed;
			}
			gridPatientClones.EndUpdate();
			//The grid has finished refreshing and can now have it's selected index changed.
			if(selectedIndex>-1) {
				gridPatientClones.SetSelected(selectedIndex,true);
			}
		}

		///<summary>Returns a boolean based on if the current state of the Family module is ready for acting on behalf of the clone feature.
		///If something is not ready for clone action to be taken a message will show to the user and false will be returned.</summary>
		private bool IsValidForCloneAction() {
			if(_patient==null) {
				MsgBox.Show(this,"Select a patient to perform clone actions.");
				return false;
			}
			return true;
		}

		///<summary></summary>
		private void ToolButAddClone_Click() {
			if(!IsValidForCloneAction()) {
				return;
			}
			FormCloneAdd formCloneAdd;
			//Check to see if the currently selected patient is a clone instead of the original or master patient.
			if(PatientLinks.IsPatientAClone(_patient.PatNum)) {
				long patNumMaster=PatientLinks.GetOriginalPatNumFromClone(_patient.PatNum);
				Patient patientMaster=Patients.GetPat(patNumMaster);
				//Double check that the original or master patient was found.
				if(patientMaster==null) {
					MsgBox.Show(this,"The original patient cannot be found in order to create additional clones.  Please call support.");
					return;
				}
				formCloneAdd=new FormCloneAdd(patientMaster);
			}
			else {//The currently selected patient is the original or master patient.
				formCloneAdd=new FormCloneAdd(_patient,_family,_listInsPlans,_listInsSubs,_listBenefits);
			}
			formCloneAdd.ShowDialog();
			//At this point we know that we have all information regarding the original or master patient.
			if(formCloneAdd.DialogResult!=DialogResult.OK) {
				return;
			}
			//Refresh the module with the new clone if one was created.
			long patNum=_patient.PatNum;
			if(formCloneAdd.PatNumClone>0) {
				patNum=formCloneAdd.PatNumClone;
			}
			formCloneAdd.Dispose();
			ModuleSelected(patNum);
		}

		///<summary></summary>
		private void ToolButBreakClone_Click() {
			if(!IsValidForCloneAction()) {
				return;
			}
			if(PatientLinks.IsPatientAClone(_patient.PatNum)) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Break the currently selected clone from the current clone group?")) {
					return;
				}
				PatientLinks.DeletePatNumTos(_patient.PatNum,PatientLinkType.Clone);
				ModuleSelected(_patient.PatNum);
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"The original patient clone is currently selected.  "
				+"Breaking the original patient clone will cause all clone links in the current clone group to be broken.\r\n"
				+"Continue anyway?")) 
			{
				return;
			}
			PatientLinks.DeletePatNumFroms(_patient.PatNum,PatientLinkType.Clone);
			ModuleSelected(_patient.PatNum);
		}

		///<summary></summary>
		private void ToolButSynchClone_Click() {
			if(!IsValidForCloneAction()) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Demographic and Insurance Plan information from the selected patient will get synchronized to all clones of this patient.\r\n"
				+"Continue?"))
			{
				return;
			}
			string strDataUpdated=Patients.SynchClonesWithPatient(_patient,_family,_listInsPlans,_listInsSubs,_listBenefits,_listPatPlans);
			ModuleSelected(_patient.PatNum);
			if(string.IsNullOrWhiteSpace(strDataUpdated)) {
				strDataUpdated=Lan.g(this,"No changes were made, data already in sync.");
			}
			new MsgBoxCopyPaste(strDataUpdated).Show();
		}
		#endregion Methods - Private - Patient Clones

		#region Methods - Private - Other
		private void FillPatientPicture() {
			if(Plugins.HookMethod(this,"ContrFamily.FillPatientPicture",_patient,pictureBoxPat)){
				return;
			}
			pictureBoxPat.Image?.Dispose();
			pictureBoxPat.Image=null;
			pictureBoxPat.TextNullImage=Lan.g(this,"Patient Picture Unavailable");
			if(_patient==null || 
				PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {//Do not use patient image when A to Z folders are disabled.
				return;
			}
			pictureBoxPat.Image?.Dispose();
			string patFolder="";
			try{
			 patFolder=ImageStore.GetPatientFolder(_patient,ImageStore.GetPreferredAtoZpath());
			}
			catch{ }
			if(patFolder==""){
				return;
			}
			if(_loadData.HasPatPict==YN.Unknown) {
				pictureBoxPat.Image=Documents.GetPatPict(_patient.PatNum,patFolder);
			}
			else {
				pictureBoxPat.Image=Documents.GetPatPict(_patient.PatNum,patFolder,_loadData.PatPict);
			}
		}

		///<summary>Shows warning if the patient's guarantor has been sent to TSI. Returns true if the patient has been sent to TSI and the user wants to cancel the move. Otherwise, returns false.</summary>
		private bool IsGuarantorTSI() {
			if(!TsiTransLogs.HasGuarBeenSentToTSI(_family.Guarantor)) {
				return false;
			}
			return !MsgBox.Show(this,MsgBoxButtons.OKCancel,"The guarantor of this family has been sent to TSI for a past due balance. "
				+"Moving a family member could change the balance and result in a charge by TSI. "
				+"We recommend canceling TSI professional collection before moving a family member.\r\nContinue with the move?");
		}

		private bool MovePats(Patient patientOld,Family family=null) {
			//no need to check insurance.  It will follow.
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Moving the guarantor will cause two families to be combined.  The financial notes for both families will be combined and may need to be edited.  The address notes will also be combined and may need to be edited. Do you wish to continue?")) {
				return false;
			}
			if(IsGuarantorTSI()) {
				return false;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Select the family to move this patient to from the list that will come up next.")) {
				return false;
			}
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.IsSelectionModeOnly=true;
			formPatientSelect.ShowDialog();
			if(formPatientSelect.DialogResult!=DialogResult.OK) {
				return false;
			}
			Patient patientInNewFam=Patients.GetPat(formPatientSelect.PatNumSelected);
			if(family!=null) {//Move all family members marked as merged silently (The family should only contain guarantor and any merged pats at this point)
				for(int i=0;i<family.ListPats.Count();i++) {
					if(family.ListPats[i].PatNum==patientOld.PatNum) {
						continue;//Don't move current pat yet
					}
					Patient patientOldFam=family.ListPats[i].Copy();
					family.ListPats[i].Guarantor=patientInNewFam.Guarantor;
					family.ListPats[i].SuperFamily=patientInNewFam.SuperFamily;
					Patients.Update(family.ListPats[i],patientOldFam);
				}
			}
			if(_patient.SuperFamily!=patientInNewFam.SuperFamily) {//If they are moving into or out of a superfamily
				if(_patient.SuperFamily!=0) {//If they are currently in a SuperFamily and moving out.  Otherwise, no superfamily popups to worry about.
					Popups.CopyForMovingSuperFamily(_patient,patientInNewFam.SuperFamily);
				}
			}
			_patient.Guarantor=patientInNewFam.Guarantor;
			_patient.SuperFamily=patientInNewFam.SuperFamily;
			Patients.Update(_patient,patientOld);
			_family=Patients.GetFamily(_patient.PatNum);
			Patients.CombineGuarantors(_family,_patient);
			return true;
		}

		private void RefreshModuleData(long patNum) {
			if(patNum==0)	{
				_patient=null;
				_patientNote=null;
				_discountPlanSub=null;
				_family=null;
				_listPatPlans=new List<PatPlan>(); 
				return;
			}
			if(_patient!=null && _discountPlanSub!=null && _loadData.ListPatPlans.Count>0) {
				DiscountPlanSubs.DeleteForPatient(_patient.PatNum);
				string logText=Lan.g(this,"The discount plan")+" "+DiscountPlans.GetPlan(_discountPlanSub.DiscountPlanNum).Description+" "+Lan.g(this,"was automatically dropped due to patient having an insuarance plan.");
				SecurityLogs.MakeLogEntry(EnumPermType.DiscountPlanAddDrop,_patient.PatNum,logText);
				string messageText="Discount plan removed due to patient having both an insurance plan and a discount plan. "
					+"If the patient should have a discount plan, please first remove all insurance plans before adding a discount plan.";
				MsgBox.Show(messageText);
			}
			bool createSecLog=false;
			if(_patNumLast!=patNum) {
				createSecLog=true;
				_patNumLast=patNum;//Stops module from making too many logs
			}
			_loadData=FamilyModules.GetLoadData(patNum,createSecLog);
			_family=_loadData.Fam;
			_patient=_loadData.Pat;
			_discountPlanSub=_loadData.DiscountPlanSub;
			_patientNote=_loadData.PatNote;
			_listInsSubs=_loadData.ListInsSubs;
			_listInsPlans=_loadData.ListInsPlans;
			_listPatPlans=_loadData.ListPatPlans;
			_listBenefits=_loadData.ListBenefits;
			_listRecalls=_loadData.ListRecalls;
			_arrayPatFields=_loadData.ArrPatFields;
			_listPatientsSuperFamilyMembers=_loadData.SuperFamilyMembers;
			_listPatientsSuperFamilyGuarantors=_loadData.SuperFamilyGuarantors;
			//Takes the preference string and converts it to an enum object
			_sortStrategySuperFam=(SortStrategy)PrefC.GetInt(PrefName.SuperFamSortStrategy);
		}

		private void RefreshModuleScreen() {
			if(_patient!=null){//if there is a patient
				//ToolBarMain.Buttons["Recall"].Enabled=true;
				toolBarMain.Buttons["Add"].Enabled=true;
				toolBarMain.Buttons["Delete"].Enabled=true;
				toolBarMain.Buttons["Guarantor"].Enabled=true;
				toolBarMain.Buttons["Move"].Enabled=true;
				if(toolBarMain.Buttons["Ins"]!=null && !PrefC.GetBool(PrefName.EasyHideInsurance)) {
					toolBarMain.Buttons["Ins"].Enabled=true;
					toolBarMain.Buttons["Discount"].Enabled=true;
				}
				//Only show Superfamily and Patient Clone containers if either feature is on and there is information for the enabled feature to show.
				//The program will still need to be restarted to ensure all UI changes are accurately reflected.
				bool showPanelForSuperfamilies=PrefC.GetBool(PrefName.ShowFeatureSuperfamilies) && _patient.SuperFamily!=0;
				bool showPanelForPatientClone=PrefC.GetBool(PrefName.ShowFeaturePatientClone) && _loadData.ListPatientsClones!=null && _loadData.ListPatientsClones.Count > 1;
				if(showPanelForSuperfamilies || showPanelForPatientClone) {
					splitContainerSuperClones.Visible=true;
					LayoutManager.MoveLocation(gridIns,new Point(splitContainerSuperClones.Right+2,gridIns.Top));
					LayoutManager.MoveWidth(gridIns,Width-gridIns.Left);
				}
				else {
					splitContainerSuperClones.Visible=false;
					LayoutManager.MoveLocation(gridIns,splitContainerSuperClones.Location);
					LayoutManager.MoveWidth(gridIns,Width-gridIns.Left);
				}
				if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies) && toolBarMain.Buttons["AddSuper"]!=null) {
					toolBarMain.Buttons["AddSuper"].Enabled=true;
				}
				if(_patient.SuperFamily==0 || toolBarMain.Buttons["AddSuper"]==null) {
					splitContainerSuperClones.Panel1Collapsed=true;
					splitContainerSuperClones.Panel1.Hide();
				}
				else {
					splitContainerSuperClones.Panel1Collapsed=false;
					splitContainerSuperClones.Panel1.Show();
					toolBarMain.Buttons["AddSuper"].Enabled=true;
					toolBarMain.Buttons["RemoveSuper"].Enabled=true;
					toolBarMain.Buttons["DisbandSuper"].Enabled=true;
				}
				if(PrefC.GetBool(PrefName.ShowFeaturePatientClone)
					&& toolBarMain.Buttons["AddClone"]!=null)
				{
					toolBarMain.Buttons["AddClone"].Enabled=true;
				}
				if(_loadData.ListPatientsClones!=null && _loadData.ListPatientsClones.Count>1
					&& Patients.IsPatientACloneOrOriginal(_patient.PatNum)
					&& toolBarMain.Buttons["SynchClone"]!=null
					&& toolBarMain.Buttons["BreakClone"]!=null)
				{
					toolBarMain.Buttons["SynchClone"].Enabled=true;
					toolBarMain.Buttons["BreakClone"].Enabled=true;
					splitContainerSuperClones.Panel2Collapsed=false;
					splitContainerSuperClones.Panel2.Show();
				}
				else {
					splitContainerSuperClones.Panel2Collapsed=true;
					splitContainerSuperClones.Panel2.Hide();
					if(toolBarMain.Buttons["SynchClone"]!=null && toolBarMain.Buttons["BreakClone"]!=null) {
						toolBarMain.Buttons["SynchClone"].Enabled=false;
						toolBarMain.Buttons["BreakClone"].Enabled=false;
					}
				}
				toolBarMain.Invalidate();
			}
			else {//no patient selected
				//Hide super family and patient clone grids, safe to run even if panel is already hidden.
				splitContainerSuperClones.Visible=false;
				LayoutManager.MoveLocation(gridIns,splitContainerSuperClones.Location);
				LayoutManager.MoveWidth(gridIns,Width-gridIns.Left);
				toolBarMain.Buttons["Add"].Enabled=false;
				toolBarMain.Buttons["Delete"].Enabled=false;
				toolBarMain.Buttons["Guarantor"].Enabled=false;
				toolBarMain.Buttons["Move"].Enabled=false;
				if(toolBarMain.Buttons["AddSuper"]!=null) {//because the toolbar only refreshes on restart.
					toolBarMain.Buttons["AddSuper"].Enabled=false;
					toolBarMain.Buttons["RemoveSuper"].Enabled=false;
					toolBarMain.Buttons["DisbandSuper"].Enabled=false;
				}
				if(toolBarMain.Buttons["Ins"]!=null && !PrefC.GetBool(PrefName.EasyHideInsurance)) {
					toolBarMain.Buttons["Ins"].Enabled=false;
					toolBarMain.Buttons["Discount"].Enabled=false;
				}
				if(PrefC.GetBool(PrefName.ShowFeaturePatientClone)
					&& toolBarMain.Buttons["AddClone"]!=null
					&& toolBarMain.Buttons["SynchClone"]!=null
					&& toolBarMain.Buttons["BreakClone"]!=null)
				{
					toolBarMain.Buttons["AddClone"].Enabled=false;
					toolBarMain.Buttons["SynchClone"].Enabled=false;
					toolBarMain.Buttons["BreakClone"].Enabled=false;
				}
				toolBarMain.Invalidate();
			}
			if(PrefC.GetBool(PrefName.EasyHideInsurance)) {
				gridIns.Visible=false;
			}
			else {
				gridIns.Visible=true;
			}
			//Cannot add new patients from OD select patient interface.  Patient must be added from HL7 message.
			if(HL7Defs.IsExistingHL7Enabled()) {
				HL7Def hL7Def=HL7Defs.GetOneDeepEnabled();
				if(hL7Def.ShowDemographics!=HL7ShowDemographics.ChangeAndAdd) {
					toolBarMain.Buttons["Add"].Enabled=false;
					toolBarMain.Buttons["Delete"].Enabled=false;
					if(PrefC.GetBool(PrefName.ShowFeaturePatientClone)
						&& toolBarMain.Buttons["AddClone"]!=null
						&& toolBarMain.Buttons["SynchClone"]!=null
						&& toolBarMain.Buttons["BreakClone"]!=null)
					{
						toolBarMain.Buttons["AddClone"].Enabled=false;
						toolBarMain.Buttons["SynchClone"].Enabled=false;
						toolBarMain.Buttons["BreakClone"].Enabled=false;
					}
				}
			}
			else {
				if(Programs.UsingEcwFullMode()) {
					toolBarMain.Buttons["Add"].Enabled=false;
					toolBarMain.Buttons["Delete"].Enabled=false;
					if(PrefC.GetBool(PrefName.ShowFeaturePatientClone)
						&& toolBarMain.Buttons["AddClone"]!=null
						&& toolBarMain.Buttons["SynchClone"]!=null
						&& toolBarMain.Buttons["BreakClone"]!=null)
					{
						toolBarMain.Buttons["AddClone"].Enabled=false;
						toolBarMain.Buttons["SynchClone"].Enabled=false;
						toolBarMain.Buttons["BreakClone"].Enabled=false;
					}
				}
			}	
			LayoutManager.LayoutControlBoundsAndFonts(splitContainerSuperClones);
			FillPatientPicture();
			FillPatientData();
			FillFamilyData();
			FillGridRecall();
			FillInsData();
			FillGridSuperFam();
			FillGridPatientClones();
			Plugins.HookAddCode(this,"ContrFamily.RefreshModuleScreen_end");
		} 
		#endregion Methods - Private - Other

		#region Methods - Helpers - GridIns
		//Helper method that updates all patplan ordinals for the patient that are in ordinal-conflict with the most recently closed PatPlan
		private void UpdateConflictingOrdinals(PatPlan patPlan) {
			byte byteNonConflictingOrdinalNum=1;
			List<PatPlan> listPatPlansConflictingOrdinal=_listPatPlans.Where(x => x.Ordinal==patPlan.Ordinal && x.PatPlanNum!=patPlan.PatPlanNum).ToList();
			for(int i=0;i<listPatPlansConflictingOrdinal.Count();i++) {
				//Incrementing until we reach the first unused Ordinal
				while(byteNonConflictingOrdinalNum<=Byte.MaxValue && _listPatPlans.Count(x => x.Ordinal==byteNonConflictingOrdinalNum)>0) {
					byteNonConflictingOrdinalNum++;
					if(byteNonConflictingOrdinalNum==Byte.MaxValue) {
						break; //This should never happen, but I know that if I don't put this here someone will hit it eventually
					}
				}
				listPatPlansConflictingOrdinal[i].Ordinal=byteNonConflictingOrdinalNum;
				PatPlans.Update(listPatPlansConflictingOrdinal[i]); //This should, in a normal scenario, only run once or twice
			}
		}
		#endregion Methods - Helpers - GridIns
	}
}
