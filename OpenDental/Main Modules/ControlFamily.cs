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
	public class ControlFamily : UserControl {
		#region Fields - Public
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		#endregion Fields - Public

		#region Fields - Private
		private PatField[] _arrayPatFields;
		///<summary>Filled with all clones for the currently selected patient and their corresponding specialty.
		///Specialties are only important if clinics are enabled.  If clinics are disabled then the corresponding Def will be null.</summary>
		private Dictionary<Patient,Def> _dictCloneSpecialty;
		private DiscountPlanSub _discountPlanSubCur;
		private Family _famCur;
		private GridOD _gridFamily;
		private GridOD _gridIns;
		private GridOD _gridPat;
		private GridOD _gridPatientClones;
		private GridOD _gridRecall;
		private GridOD _gridSuperFam;	
		private IContainer _iComponents;
		private ImageList _imageListToolBar;
		private bool _initializedOnStartup;
		private List <Benefit> _listBenefits;
		private List <InsPlan> _listInsPlans;
		private List<InsSub> _listInsSubs;
		private List <PatPlan> _listPatPlans;
		private List<Patient> _listPatSuperFamilyGuarantors;
		private List<Patient> _listPatSuperFamilyMembers;
		///<summary>All recalls for this entire family.</summary>
		private List<Recall> _listRecalls;
		///<summary>All the data necessary to load the module.</summary>
		private FamilyModules.LoadData _loadData;
		private ContextMenu _menuDiscount;
		private ContextMenu _menuInsurance;
		private MenuItem _menuItemRemoveDiscount;
		private MenuItem _menuItemPlansForFam;
		private ODPictureBox _odPictureBoxPat;
		private Patient _patCur;
		private PatientNote _patNoteCur;
		///<summary>Gets updated to PatCur.PatNum that the last security log was made with so that we don't make too many security logs for this patient.  When _patNumLast no longer matches PatCur.PatNum (e.g. switched to a different patient within a module), a security log will be entered.  Gets reset (cleared and the set back to PatCur.PatNum) any time a module button is clicked which will cause another security log to be entered.</summary>
		private long _patNumLast;
		///<summary>Used for MenuItemPopup() to tell which row the user clicked on.  Currently only for gridPat</summary>
		private Point _pointLastClicked;
		private SplitContainer _splitContainerSuperClones;
		private SortStrategy _sortStratSuperFam;
		private ToolBarOD _toolBarMain;
		#endregion Fields - Private

		#region Constructor
		///<summary></summary>
		public ControlFamily(){
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.Start);
			InitializeComponent();// This call is required by the Windows.Forms Form Designer.
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.End);
		}
		#endregion Constructor

		#region Dispose
		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(_iComponents != null){
					_iComponents.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion Dispose

		#region Component Designer generated code
		private void InitializeComponent(){
			this._iComponents = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlFamily));
			this._imageListToolBar = new System.Windows.Forms.ImageList(this._iComponents);
			this._menuInsurance = new System.Windows.Forms.ContextMenu();
			this._menuItemPlansForFam = new System.Windows.Forms.MenuItem();
			this._menuDiscount = new System.Windows.Forms.ContextMenu();
			this._menuItemRemoveDiscount = new System.Windows.Forms.MenuItem();
			this._gridSuperFam = new OpenDental.UI.GridOD();
			this._gridRecall = new OpenDental.UI.GridOD();
			this._gridFamily = new OpenDental.UI.GridOD();
			this._gridPat = new OpenDental.UI.GridOD();
			this._gridIns = new OpenDental.UI.GridOD();
			this._splitContainerSuperClones = new System.Windows.Forms.SplitContainer();
			this._gridPatientClones = new OpenDental.UI.GridOD();
			this._odPictureBoxPat = new OpenDental.UI.ODPictureBox();
			this._toolBarMain = new OpenDental.UI.ToolBarOD();
			((System.ComponentModel.ISupportInitialize)(this._splitContainerSuperClones)).BeginInit();
			this._splitContainerSuperClones.Panel1.SuspendLayout();
			this._splitContainerSuperClones.Panel2.SuspendLayout();
			this._splitContainerSuperClones.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageListToolBar
			// 
			this._imageListToolBar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListToolBar.ImageStream")));
			this._imageListToolBar.TransparentColor = System.Drawing.Color.Transparent;
			this._imageListToolBar.Images.SetKeyName(0, "");
			this._imageListToolBar.Images.SetKeyName(1, "");
			this._imageListToolBar.Images.SetKeyName(2, "");
			this._imageListToolBar.Images.SetKeyName(3, "");
			this._imageListToolBar.Images.SetKeyName(4, "");
			this._imageListToolBar.Images.SetKeyName(5, "");
			this._imageListToolBar.Images.SetKeyName(6, "Umbrella.gif");
			// 
			// menuInsurance
			// 
			this._menuInsurance.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._menuItemPlansForFam});
			// 
			// menuPlansForFam
			// 
			this._menuItemPlansForFam.Index = 0;
			this._menuItemPlansForFam.Text = "Plans for Family";
			this._menuItemPlansForFam.Click += new System.EventHandler(this.menuPlansForFam_Click);
			// 
			// menuDiscount
			// 
			this._menuDiscount.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._menuItemRemoveDiscount});
			// 
			// menuItemRemoveDiscount
			// 
			this._menuItemRemoveDiscount.Index = 0;
			this._menuItemRemoveDiscount.Text = "Drop Discount Plan";
			this._menuItemRemoveDiscount.Click += new System.EventHandler(this.menuItemRemoveDiscount_Click);
			// 
			// gridSuperFam
			// 
			this._gridSuperFam.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._gridSuperFam.Location = new System.Drawing.Point(0, 0);
			this._gridSuperFam.Name = "gridSuperFam";
			this._gridSuperFam.Size = new System.Drawing.Size(329, 282);
			this._gridSuperFam.TabIndex = 33;
			this._gridSuperFam.Title = "Super Family";
			this._gridSuperFam.TranslationName = "TableSuper";
			this._gridSuperFam.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridSuperFam_CellDoubleClick);
			this._gridSuperFam.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridSuperFam_CellClick);
			// 
			// gridRecall
			// 
			this._gridRecall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._gridRecall.Location = new System.Drawing.Point(585, 27);
			this._gridRecall.Name = "gridRecall";
			this._gridRecall.Size = new System.Drawing.Size(354, 100);
			this._gridRecall.TabIndex = 32;
			this._gridRecall.Title = "Recall";
			this._gridRecall.TranslationName = "TableRecall";
			this._gridRecall.WrapText = false;
			this._gridRecall.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridRecall_CellDoubleClick);
			this._gridRecall.DoubleClick += new System.EventHandler(this.gridRecall_DoubleClick);
			// 
			// gridFamily
			// 
			this._gridFamily.ColorSelectedRow = System.Drawing.Color.DarkSalmon;
			this._gridFamily.Location = new System.Drawing.Point(103, 27);
			this._gridFamily.Name = "gridFamily";
			this._gridFamily.Size = new System.Drawing.Size(480, 100);
			this._gridFamily.TabIndex = 31;
			this._gridFamily.Title = "Family Members";
			this._gridFamily.TranslationName = "TableFamily";
			this._gridFamily.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridFamily_CellDoubleClick);
			this._gridFamily.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridFamily_CellClick);
			// 
			// gridPat
			// 
			this._gridPat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this._gridPat.Location = new System.Drawing.Point(0, 129);
			this._gridPat.Name = "gridPat";
			this._gridPat.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this._gridPat.Size = new System.Drawing.Size(252, 579);
			this._gridPat.TabIndex = 30;
			this._gridPat.Title = "Patient Information";
			this._gridPat.TranslationName = "TablePatient";
			this._gridPat.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPat_CellDoubleClick);
			this._gridPat.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPat_CellClick);
			this._gridPat.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridPat_MouseDown);
			// 
			// gridIns
			// 
			this._gridIns.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._gridIns.HScrollVisible = true;
			this._gridIns.Location = new System.Drawing.Point(254, 129);
			this._gridIns.Name = "gridIns";
			this._gridIns.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this._gridIns.Size = new System.Drawing.Size(685, 579);
			this._gridIns.TabIndex = 29;
			this._gridIns.Title = "Insurance Plans";
			this._gridIns.TranslationName = "TableCoverage";
			this._gridIns.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridIns_CellDoubleClick);
			// 
			// splitContainerSuperClones
			// 
			this._splitContainerSuperClones.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this._splitContainerSuperClones.Location = new System.Drawing.Point(254, 129);
			this._splitContainerSuperClones.Name = "splitContainerSuperClones";
			this._splitContainerSuperClones.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerSuperClones.Panel1
			// 
			this._splitContainerSuperClones.Panel1.Controls.Add(this._gridSuperFam);
			// 
			// splitContainerSuperClones.Panel2
			// 
			this._splitContainerSuperClones.Panel2.Controls.Add(this._gridPatientClones);
			this._splitContainerSuperClones.Size = new System.Drawing.Size(329, 579);
			this._splitContainerSuperClones.SplitterDistance = 285;
			this._splitContainerSuperClones.TabIndex = 34;
			this._splitContainerSuperClones.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainerSuperClones_SplitterMoved);
			// 
			// gridPatientClones
			// 
			this._gridPatientClones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._gridPatientClones.Location = new System.Drawing.Point(0, 0);
			this._gridPatientClones.Name = "gridPatientClones";
			this._gridPatientClones.Size = new System.Drawing.Size(329, 290);
			this._gridPatientClones.TabIndex = 34;
			this._gridPatientClones.Title = "Patient Clones";
			this._gridPatientClones.TranslationName = "TablePatientClones";
			this._gridPatientClones.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPatientClone_CellClick);
			// 
			// picturePat
			// 
			this._odPictureBoxPat.Location = new System.Drawing.Point(1, 27);
			this._odPictureBoxPat.Name = "picturePat";
			this._odPictureBoxPat.Size = new System.Drawing.Size(100, 100);
			this._odPictureBoxPat.TabIndex = 28;
			this._odPictureBoxPat.Text = "picturePat";
			this._odPictureBoxPat.TextNullImage = "Patient Picture Unavailable";
			// 
			// ToolBarMain
			// 
			this._toolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this._toolBarMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this._toolBarMain.ImageList = this._imageListToolBar;
			this._toolBarMain.Location = new System.Drawing.Point(0, 0);
			this._toolBarMain.Name = "ToolBarMain";
			this._toolBarMain.Size = new System.Drawing.Size(939, 25);
			this._toolBarMain.TabIndex = 19;
			this._toolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.ToolBarMain_ButtonClick);
			// 
			// ControlFamily
			// 
			this.Controls.Add(this._splitContainerSuperClones);
			this.Controls.Add(this._gridRecall);
			this.Controls.Add(this._gridFamily);
			this.Controls.Add(this._gridPat);
			this.Controls.Add(this._gridIns);
			this.Controls.Add(this._odPictureBoxPat);
			this.Controls.Add(this._toolBarMain);
			this.Name = "ControlFamily";
			this.Size = new System.Drawing.Size(939, 708);
			this._splitContainerSuperClones.Panel1.ResumeLayout(false);
			this._splitContainerSuperClones.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._splitContainerSuperClones)).EndInit();
			this._splitContainerSuperClones.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Methods - Event Handlers - GridPatient
		private void gridPat_CellClick(object sender,ODGridClickEventArgs e) {
			GridCell gridCellCur=_gridPat.ListGridRows[e.Row].Cells[e.Col];
			//Only grid cells with phone numbers are blue and underlined. 
			//If we support color and underline in the future, this might be changed to a regex of the cell text.
			if(gridCellCur.ColorText==System.Drawing.Color.Blue && gridCellCur.Underline==YN.Yes && Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
				DentalTek.PlaceCall(gridCellCur.Text);
			}
		}

		private void gridPat_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(Plugins.HookMethod(this,"ContrFamily.gridPat_CellDoubleClick",_patCur)) {
				return;
			}
			if(TerminalActives.PatIsInUse(_patCur.PatNum)) {
				MsgBox.Show(this,"Patient is currently entering info at a reception terminal.  Please try again later.");
				return;
			}
			if(_gridPat.ListGridRows[e.Row].Tag==null 
				|| _gridPat.ListGridRows[e.Row].Tag.ToString()=="SS#"
				|| _gridPat.ListGridRows[e.Row].Tag.ToString()=="DOB") 
			{
				if(!Security.IsAuthorized(Permissions.PatientEdit)) {
					return;
				}
				using FormPatientEdit formPatientEdit=new FormPatientEdit(_patCur,_famCur);
				formPatientEdit.IsNew=false;
				formPatientEdit.ShowDialog();
				if(formPatientEdit.DialogResult==DialogResult.OK) {
					FormOpenDental.S_Contr_PatientSelected(_patCur,false);
				}
			}
			//Check tags and perform corresponding action for said tag type.
			else if(_gridPat.ListGridRows[e.Row].Tag.ToString()=="Referral") {
				//RefAttach refattach=(RefAttach)gridPat.Rows[e.Row].Tag;
				using FormReferralsPatient formReferralsPatient=new FormReferralsPatient();
				formReferralsPatient.PatNum=_patCur.PatNum;
				formReferralsPatient.ShowDialog();
			}
			else if(_gridPat.ListGridRows[e.Row].Tag.ToString()=="References") {
				using FormReference formReference=new FormReference();
				formReference.ShowDialog();
				if(formReference.GotoPatNum!=0) {
					Patient pat=Patients.GetPat(formReference.GotoPatNum);
					FormOpenDental.S_Contr_PatientSelected(pat,false);
					GotoModule.GotoFamily(formReference.GotoPatNum);
					return;
				}
				if(formReference.DialogResult!=DialogResult.OK) {
					return;
				}
				for(int i=0;i<formReference.SelectedCustRefs.Count;i++) {
					CustRefEntry custRefEntry=new CustRefEntry();
					custRefEntry.DateEntry=DateTime.Now;
					custRefEntry.PatNumCust=_patCur.PatNum;
					custRefEntry.PatNumRef=formReference.SelectedCustRefs[i].PatNum;
					CustRefEntries.Insert(custRefEntry);
				}
			}
			else if(_gridPat.ListGridRows[e.Row].Tag.GetType()==typeof(CustRefEntry)) {
				using FormReferenceEntryEdit formRefEntryEdit=new FormReferenceEntryEdit((CustRefEntry)_gridPat.ListGridRows[e.Row].Tag);
				formRefEntryEdit.ShowDialog();
			}
			else if(_gridPat.ListGridRows[e.Row].Tag.ToString().Equals("Payor Types")) {
				using FormPayorTypes formPayorTypes=new FormPayorTypes();
				formPayorTypes.PatCur=_patCur;
				formPayorTypes.ShowDialog();
			}
			else if(_gridPat.ListGridRows[e.Row].Tag is PatFieldDef) {//patfield for an existing PatFieldDef
				PatFieldDef patFieldDef=(PatFieldDef)_gridPat.ListGridRows[e.Row].Tag;
				PatField patField=PatFields.GetByName(patFieldDef.FieldName,_arrayPatFields);
				PatFieldL.OpenPatField(patField,patFieldDef,_patCur.PatNum);
			}
			else if(_gridPat.ListGridRows[e.Row].Tag is PatField) {//PatField for a PatFieldDef that no longer exists
				PatField patField=(PatField)_gridPat.ListGridRows[e.Row].Tag;
				using FormPatFieldEdit formPatFieldEdit=new FormPatFieldEdit(patField);
				formPatFieldEdit.ShowDialog();
			}
			else if(_gridPat.ListGridRows[e.Row].Tag is Address) {
				Address address=(Address)_gridPat.ListGridRows[e.Row].Tag;
				if(address.IsNew) { //add the patCur's patNum is new
					address.PatNumTaxPhysical=_patCur.PatNum;
				}
				using FormTaxAddress formTaxAddress=new FormTaxAddress();
				formTaxAddress.AddressCur=address;
				formTaxAddress.PatCur=_patCur;
				formTaxAddress.ShowDialog();
			}
			ModuleSelected(_patCur.PatNum);
		}

		private void gridPat_MouseDown(object sender,MouseEventArgs e) {
			_pointLastClicked=e.Location;
		}

		///<summary>Just prior to displaying the context menu, enable or disables the UnmaskDOB option</summary>
		private void MenuItemPopupUnmaskDOB(object sender,EventArgs e) {
			MenuItem menuItemDOB=_gridPat.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Name=="ViewDOB");
			if(menuItemDOB==null) { 
				return;//Should not happen
			}
			MenuItem menuItemSeperator=_gridPat.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Text=="-");
			if(menuItemSeperator==null) { 
				return;//Should not happen
			}
			int idxRowClick=_gridPat.PointToRow(_pointLastClicked.Y);
			int idxColClick=_gridPat.PointToCol(_pointLastClicked.X);//Make sure the user clicked within the bounds of the grid.
			if(idxRowClick>-1 && idxColClick>-1 && (_gridPat.ListGridRows[idxRowClick].Tag!=null) 
				&& _gridPat.ListGridRows[idxRowClick].Tag is string
				&& ((string)_gridPat.ListGridRows[idxRowClick].Tag=="DOB"))
			{
				if(Security.IsAuthorized(Permissions.PatientDOBView,true)
					&& _gridPat.ListGridRows[idxRowClick].Cells[_gridPat.ListGridRows[idxRowClick].Cells.Count-1].Text!="")
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
			}
			else {
				menuItemDOB.Visible=false;
				menuItemDOB.Enabled=false;
				if(_gridPat.ContextMenu.MenuItems.OfType<MenuItem>().Count(x => x.Visible==true && x.Text!="-")>1) {
					//There is more than one item showing, we want the seperator.
					menuItemSeperator.Visible=true;
					menuItemSeperator.Enabled=true;
				}
				else {
					//We dont want the seperator to be there with only one option.
					menuItemSeperator.Visible=false;
					menuItemSeperator.Enabled=false;
				}
			}
		}

		///<summary>Just prior to displaying the context menu, enable or disables the UnmaskSSN option</summary>
		private void MenuItemPopupUnmaskSSN(object sender,EventArgs e) {
			MenuItem menuItemSSN=_gridPat.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Name=="ViewSS#");
			if(menuItemSSN==null) { 
				return;//Should not happen
			}
			MenuItem menuItemSeperator=_gridPat.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Text=="-");
			if(menuItemSeperator==null) {
				return;//Should not happen
			}
			int idxRowClick=_gridPat.PointToRow(_pointLastClicked.Y);
			int idxColClick=_gridPat.PointToCol(_pointLastClicked.X);//Make sure the user clicked within the bounds of the grid.
			if(idxRowClick>-1 && idxColClick>-1 && (_gridPat.ListGridRows[idxRowClick].Tag!=null) 
				&& _gridPat.ListGridRows[idxRowClick].Tag is string
				&& ((string)_gridPat.ListGridRows[idxRowClick].Tag=="SS#"))
			{
				if(Security.IsAuthorized(Permissions.PatientSSNView,true) 
					&& _gridPat.ListGridRows[idxRowClick].Cells[_gridPat.ListGridRows[idxRowClick].Cells.Count-1].Text!="")
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
			}
			else {
				menuItemSSN.Visible=false;
				menuItemSSN.Enabled=false;
				if(_gridPat.ContextMenu.MenuItems.OfType<MenuItem>().Count(x => x.Visible==true && x.Text!="-")>1) {
					//There is more than one item showing, we want the seperator.
					menuItemSeperator.Visible=true;
					menuItemSeperator.Enabled=true;
				}
				else {
					//We dont want the seperator to be there with only one option.
					menuItemSeperator.Visible=false;
					menuItemSeperator.Enabled=false;
				}
			}
		}

		private void MenuItemUnmaskDOB_Click(object sender,EventArgs e) {
			//Preference and permissions check has already happened by this point.
			//Guaranteed to be clicking on a valid row & column.
			int rowClick=_gridPat.PointToRow(_pointLastClicked.Y);
			_gridPat.BeginUpdate();
			GridRow row=_gridPat.ListGridRows[rowClick];
			row.Cells[row.Cells.Count-1].Text=Patients.DOBFormatHelper(_patCur.Birthdate,false);
			_gridPat.EndUpdate();
			string logtext="Date of birth unmasked in Family Module";
			SecurityLogs.MakeLogEntry(Permissions.PatientDOBView,_patCur.PatNum,logtext);
		}

		private void MenuItemUnmaskSSN_Click(object sender,EventArgs e) {
			//Preference and permissions check has already happened by this point.
			//Guaranteed to be clicking on a valid row & column.
			int rowClick=_gridPat.PointToRow(_pointLastClicked.Y);
			_gridPat.BeginUpdate();
			GridRow row=_gridPat.ListGridRows[rowClick];
			row.Cells[row.Cells.Count-1].Text=Patients.SSNFormatHelper(_patCur.SSN,false);
			_gridPat.EndUpdate();
			string logtext="";
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				logtext="Social Insurance Number";
			}
			else {
				logtext="Social Security Number";
			}
			logtext+=" unmasked in Family Module";
			SecurityLogs.MakeLogEntry(Permissions.PatientSSNView,_patCur.PatNum,logtext);
		}
		#endregion Methods - Event Handlers - GridPatient 

		#region Methods - Event Handlers - GridFamily
		private void gridFamily_CellClick(object sender,ODGridClickEventArgs e) {
			FormOpenDental.S_Contr_PatientSelected(_gridFamily.SelectedTag<Patient>(),false);
			ModuleSelected(_gridFamily.SelectedTag<Patient>().PatNum);
		}

		private void gridFamily_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!Security.IsAuthorized(Permissions.PatientEdit)) {
				return;
			}
			using FormPatientEdit formPatientEdit=new FormPatientEdit(_patCur,_famCur);
			formPatientEdit.IsNew=false;
			formPatientEdit.ShowDialog();
			if(formPatientEdit.DialogResult==DialogResult.OK) {
				FormOpenDental.S_Contr_PatientSelected(_patCur,false);
			}
			ModuleSelected(_patCur.PatNum);
		}
		#endregion Methods - Event Handlers - GridFamily

		#region Methods - Event Handlers - GridRecall
		private void gridRecall_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//use doubleclick instead
		}

		private void gridRecall_DoubleClick(object sender,EventArgs e) {
			if(_patCur==null) {
				return;
			}
			using FormRecallsPat formRecallPat=new FormRecallsPat();
			formRecallPat.PatNum=_patCur.PatNum;
			formRecallPat.ShowDialog();
			ModuleSelected(_patCur.PatNum);
		}
		#endregion Methods - Event Handlers - GridRecall

		#region Methods - Event Handlers - GridSuperFam
		private void gridSuperFam_CellClick(object sender,ODGridClickEventArgs e) {
			FormOpenDental.S_Contr_PatientSelected(_listPatSuperFamilyGuarantors[e.Row],false);
			ModuleSelected(_listPatSuperFamilyGuarantors[e.Row].PatNum);
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
			if(_discountPlanSubCur!=null) {
				DiscountPlanSub discountPlanSub=DiscountPlanSubs.GetSubForPat(_patCur.PatNum);
				if(discountPlanSub==null) {
					MsgBox.Show(this,"Discount plan removed by another user.");
					ModuleSelected(_patCur.PatNum);
					return;
				}
				DiscountPlan discountPlan=DiscountPlans.GetPlan(discountPlanSub.DiscountPlanNum);
				if(discountPlan==null) {
					MsgBox.Show(this,"Discount plan deleted by another user.");
					ModuleSelected(_patCur.PatNum);
					return;
				}
				if(discountPlan.DiscountPlanNum!=_discountPlanSubCur.DiscountPlanNum) {
					MsgBox.Show(this,"Discount plan changed by another user.");
					ModuleSelected(_patCur.PatNum);
					return;
				}
				using FormDiscountPlanSubEdit formDiscountPlanSubEdit=new FormDiscountPlanSubEdit();
				formDiscountPlanSubEdit.DiscountPlanSubCur=discountPlanSub;
				if(formDiscountPlanSubEdit.ShowDialog()==DialogResult.OK) {
					_discountPlanSubCur=formDiscountPlanSubEdit.DiscountPlanSubCur;
					if(_discountPlanSubCur==null) {
						TreatPlans.UpdateTreatmentPlanType(_patCur);
					}
					ModuleSelected(_patCur.PatNum);
				}
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
			_listInsSubs=InsSubs.RefreshForFam(_famCur);
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
			ModuleSelected(_patCur.PatNum);//Should refresh insplans to display new information
		}

		private void menuItemRemoveDiscount_Click(object sender,EventArgs e) {
			if(_discountPlanSubCur!=null) {
				DiscountPlanSubs.UpdateAssociatedDiscountPlanAmts(new List<DiscountPlanSub>{ _discountPlanSubCur },true);
				if(!string.IsNullOrWhiteSpace(_discountPlanSubCur.SubNote) && MsgBox.Show(MsgBoxButtons.YesNo,Lan.g("Commlogs","Save Subscriber Note to Commlog?"))) {
					Commlog commlog=new Commlog();
					commlog.PatNum=_discountPlanSubCur.PatNum;
					commlog.CommDateTime=DateTime.Now;
					commlog.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.MISC);
					commlog.Note=Lans.g("Commlogs","Subscriber note from dropped discount plan, saved copy")+": ";
					commlog.Note+=_discountPlanSubCur.SubNote;
					commlog.UserNum=Security.CurUser.UserNum;
					Commlogs.Insert(commlog);
				}
				DiscountPlanSubs.Delete(_discountPlanSubCur.DiscountSubNum);
				string logText="The discount plan "+DiscountPlans.GetPlan(_discountPlanSubCur.DiscountPlanNum).Description+" was dropped.";
				SecurityLogs.MakeLogEntry(Permissions.DiscountPlanAddDrop,_patCur.PatNum,logText);
				_discountPlanSubCur=null;
			}
			FillInsData();
		}

		private void menuPlansForFam_Click(object sender,EventArgs e) {
			using FormPlansForFamily formPlansForFamily=new FormPlansForFamily();
			formPlansForFamily.FamCur=_famCur;
			formPlansForFamily.ShowDialog();
			ModuleSelected(_patCur.PatNum);
		}
		#endregion Methods - Event Handlers - GridIns

		#region Methods - Event Handlers - Patient Clones
		private void gridPatientClone_CellClick(object sender,ODGridClickEventArgs e) {
			if(_gridPatientClones.ListGridRows[e.Row].Tag==null || _gridPatientClones.ListGridRows[e.Row].Tag.GetType()!=typeof(Patient)) {
				return;
			}
			Patient patient=(Patient)_gridPatientClones.ListGridRows[e.Row].Tag;
			FormOpenDental.S_Contr_PatientSelected(patient,false);
			ModuleSelected(patient.PatNum);
		}
		#endregion Methods - Event Handlers - Patient Clones

		#region Methods - Event Handlers - Other
		private void splitContainerSuperClones_SplitterMoved(object sender,SplitterEventArgs e) {
			LayoutManager.LayoutControlBoundsAndFonts(_splitContainerSuperClones);
		}

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
			}
			else if(e.Button.Tag.GetType()==typeof(Program)) {
				ProgramL.Execute(((Program)e.Button.Tag).ProgramNum,_patCur);
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
					_gridFamily,
					_gridRecall,
					_gridPat,
					_gridSuperFam,
					_gridIns,
				});
			LayoutToolBar();
			//gridPat.Height=this.ClientRectangle.Bottom-gridPat.Top-2;
		}

		///<summary>Causes the toolbar to be laid out again.</summary>
		public void LayoutToolBar() {
			_toolBarMain.Buttons.Clear();
			ODToolBarButton button;
			//ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Recall"),1,"","Recall"));
			//ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			button=new ODToolBarButton(Lan.g(this,"Family Members:"),-1,"","");
			button.Style=ODToolBarButtonStyle.Label;
			_toolBarMain.Buttons.Add(button);
			_toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Add"),EnumIcons.PatAdd,"Add Family Member","Add"));
			_toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Delete"),EnumIcons.PatDelete,Lan.g(this,"Delete Family Member"),"Delete"));
			_toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Set Guarantor"),EnumIcons.PatSetGuarantor,Lan.g(this,"Set as Guarantor"),"Guarantor"));
			_toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Move"),EnumIcons.PatMoveFam,Lan.g(this,"Move to Another Family"),"Move"));
			if(PrefC.GetBool(PrefName.ShowFeaturePatientClone)) {
				_toolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
				button=new ODToolBarButton(Lan.g(this,"Clones:"),-1,"","");
				button.Style=ODToolBarButtonStyle.Label;
				_toolBarMain.Buttons.Add(button);
				_toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Add"),-1,Lan.g(this,"Creates a clone of the currently selected patient."),"AddClone"));
				_toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Synch"),-1,Lan.g(this,"Synch information to the clone patient or create a clone of the currently selected patient if one does not exist"),"SynchClone"));
				_toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Break"),-1,Lan.g(this,"Remove selected patient from the clone group."),"BreakClone"));
			}
			if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
				_toolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
				button=new ODToolBarButton(Lan.g(this,"Super Family:"),-1,"","");
				button.Style=ODToolBarButtonStyle.Label;
				_toolBarMain.Buttons.Add(button);
				_toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Add"),-1,"Add selected patient to a super family","AddSuper"));
				_toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Remove"),-1,Lan.g(this,"Remove selected patient, and their family, from super family"),"RemoveSuper"));
				_toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Disband"),-1,Lan.g(this,"Disband the current super family by removing all members of the super family."),"DisbandSuper"));
			}
			if(!PrefC.GetBool(PrefName.EasyHideInsurance)) {
				_toolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
				button=new ODToolBarButton(Lan.g(this,"Add Insurance"),6,"","Ins");
				button.Style=ODToolBarButtonStyle.DropDownButton;
				button.DropDownMenu=_menuInsurance;
				_toolBarMain.Buttons.Add(button);
				_toolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
				button=new ODToolBarButton(Lan.g(this,"Discount Plan"),-1,"","Discount");
				button.Style=ODToolBarButtonStyle.DropDownButton;
				button.DropDownMenu=_menuDiscount;
				_toolBarMain.Buttons.Add(button);
			}
			ProgramL.LoadToolbar(_toolBarMain,ToolBarsAvail.FamilyModule);
			_toolBarMain.Invalidate();
			Plugins.HookAddCode(this,"ContrFamily.LayoutToolBar_end",_patCur);
		}

		///<summary></summary>
		public void ModuleSelected(long patNum) {
			RefreshModuleData(patNum);
			if(_patCur!=null && _patCur.PatStatus==PatientStatus.Deleted) {
				MsgBox.Show("Selected patient has been deleted by another workstation.");
				PatientL.RemoveFromMenu(_patCur.PatNum);
				FormOpenDental.S_Contr_PatientSelected(new Patient(),false);
				RefreshModuleData(0);
			}
			RefreshModuleScreen();
			PatientDashboardDataEvent.Fire(ODEventType.ModuleSelected,_loadData);
			Plugins.HookAddCode(this,"ContrFamily.ModuleSelected_end",patNum);
		}

		///<summary></summary>
		public void ModuleUnselected() {
			_famCur=null;
			_listInsPlans=null;
			_patNumLast=0;//Clear out the last pat num so that a security log gets entered that the module was "visited" or "refreshed".
			_gridPat.ContextMenu=new ContextMenu();//This module is never really disposed. Get rid of any menu options we added, to avoid duplicates.
			Plugins.HookAddCode(this,"ContrFamily.ModuleUnselected_end");
		}
		#endregion Methods - Public

		#region Methods - Private - GridPatient
		private void FillPatientData() {
			if(_patCur==null) {
				_gridPat.BeginUpdate();
				_gridPat.ListGridRows.Clear();
				_gridPat.Columns.Clear();
				_gridPat.EndUpdate();
				return;
			}
			if(PrefC.GetBool(PrefName.PatientSSNMasked)) {
				//Add "View SS#" right click option, MenuItemPopupUnmaskSSN will show and hide it as needed.
				if(_gridPat.ContextMenu==null) {
					_gridPat.ContextMenu=new ContextMenu();//ODGrid will automatically attach the defaut Popups
				}
				ContextMenu contextMenu=_gridPat.ContextMenu;
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
				if(_gridPat.ContextMenu==null) {
					_gridPat.ContextMenu=new ContextMenu();//ODGrid will automatically attach the defaut Popups
				}
				ContextMenu contextMenu=_gridPat.ContextMenu;
				MenuItem menuItemUnmaskDOB=new MenuItem();
				menuItemUnmaskDOB.Enabled=false;
				menuItemUnmaskDOB.Visible=false;
				menuItemUnmaskDOB.Name="ViewDOB";
				menuItemUnmaskDOB.Text="View DOB";
				menuItemUnmaskDOB.Click+= new System.EventHandler(this.MenuItemUnmaskDOB_Click);
				contextMenu.MenuItems.Add(menuItemUnmaskDOB);
				contextMenu.Popup+=MenuItemPopupUnmaskDOB;
			}
			_gridPat.BeginUpdate();
			_gridPat.Columns.Clear();
			GridColumn col=new GridColumn("",100);
			_gridPat.Columns.Add(col);
			col=new GridColumn("",150);
			_gridPat.Columns.Add(col);
			_gridPat.ListGridRows.Clear();
			GridRow row;
			List<DisplayField> listDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.PatientInformation);
			DisplayField displayFieldCur;
			List<Def> listMiscColorDefs=Defs.GetDefsForCategory(DefCat.MiscColors,true);
			for(int f=0;f<listDisplayFields.Count;f++) {
				displayFieldCur=listDisplayFields[f];
				row=new GridRow();
				#region Description Column
				if(displayFieldCur.Description=="") {
					if(displayFieldCur.InternalName=="SS#") {
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
					else if(displayFieldCur.InternalName=="State") {
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
					else if(displayFieldCur.InternalName=="Zip") {
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
					else if(displayFieldCur.InternalName=="PatFields") {
						//don't add a cell
					}
					else {
						row.Cells.Add(displayFieldCur.InternalName);
					}
				}
				else {
					if(displayFieldCur.InternalName=="PatFields") {
						//don't add a cell
					}
					else {
						row.Cells.Add(displayFieldCur.Description);
					}
				}
				#endregion Description Column
				#region Value Column
				switch(displayFieldCur.InternalName) {
					#region ABC0
					case "ABC0":
						row.Cells.Add(_patCur.CreditType);
						break;
					#endregion ABC0
					#region Addr/Ph Note
					case "Addr/Ph Note":
						row.Cells.Add(_patCur.AddrNote);
						if(_patCur.AddrNote!="") {
							row.ColorText=Color.Red;
							row.Bold=true;
						}
						break;
					#endregion Addr/Ph Note
					#region Address
					case "Address":
						row.Cells.Add(_patCur.Address);
						row.Bold=true;
						break;
					#endregion Address
					#region Address2
					case "Address2":
						row.Cells.Add(_patCur.Address2);
						break;
					#endregion Address2
					#region AdmitDate
					case "AdmitDate":
						row.Cells.Add(_patCur.AdmitDate.ToShortDateString());
						break;
					#endregion AdmitDate
					#region Age
					case "Age":
						row.Cells.Add(PatientLogic.DateToAgeString(_patCur.Birthdate,_patCur.DateTimeDeceased));
						break;
					#endregion Age
					#region Arrive Early
					case "Arrive Early":
						if(_patCur.AskToArriveEarly==0) {
							row.Cells.Add("");
						}
						else{
							row.Cells.Add(_patCur.AskToArriveEarly.ToString());
						}
						break;
					#endregion Arrive Early
					#region Billing Type
					case "Billing Type":
						string billingtype=Defs.GetName(DefCat.BillingTypes,_patCur.BillingType);
						if(Defs.GetHidden(DefCat.BillingTypes,_patCur.BillingType)) {
							billingtype+=" "+Lan.g(this,"(hidden)");
						}						
						row.Cells.Add(billingtype);
						break;
					#endregion Billing Type
					#region Birthdate
					case "Birthdate":
						if(PrefC.GetBool(PrefName.PatientDOBMasked) || !Security.IsAuthorized(Permissions.PatientDOBView,true)) {
							row.Cells.Add(Patients.DOBFormatHelper(_patCur.Birthdate,true));
							row.Tag="DOB";//Used later to tell if we're right clicking on the DOB row
						}
						else {
							row.Cells.Add(Patients.DOBFormatHelper(_patCur.Birthdate,false));
						}
						break;
					#endregion Birthdate
					#region Chart Num
					case "Chart Num":
						row.Cells.Add(_patCur.ChartNumber);
						break;
					#endregion Chart Num
					#region City
					case "City":
						row.Cells.Add(_patCur.City);
						break;
					#endregion City
					#region Clinic
					case "Clinic":
						row.Cells.Add(Clinics.GetAbbr(_patCur.ClinicNum));
						break;
					#endregion Clinic
					#region Contact Method
					case "Contact Method":
						row.Cells.Add(_patCur.PreferContactMethod.ToString());
						if(_patCur.PreferContactMethod==ContactMethod.DoNotCall || _patCur.PreferContactMethod==ContactMethod.SeeNotes) {
							row.Bold=true;
						}
						break;
					#endregion Contact Method
					#region Country
					case "Country":
						row.Cells.Add(_patCur.Country);
						break;
					#endregion Country
					#region E-mail
					case "E-mail":
						row.Cells.Add(_patCur.Email);
						if(_patCur.PreferContactMethod==ContactMethod.Email) {
							row.Bold=true;
						}
						break;
					#endregion E-mail
					#region First
					case "First":
						row.Cells.Add(_patCur.FName);
						break;
					#endregion First
					#region Gender
					case "Gender":
						row.Cells.Add(_patCur.Gender.ToString());
						break;
					#endregion Gender
					#region Guardians
					case "Guardians":
						List<Guardian> guardianList=_loadData.ListGuardians??Guardians.Refresh(_patCur.PatNum);
						string str="";
						for(int g=0;g<guardianList.Count;g++) {
							if(!guardianList[g].IsGuardian) {
								continue;
							}
							if(g>0) {
								str+=",";
							}
							str+=_famCur.GetNameInFamFirst(guardianList[g].PatNumGuardian)+Guardians.GetGuardianRelationshipStr(guardianList[g].Relationship);
						}
						row.Cells.Add(str);
						break;
					#endregion Guardians
					#region Hm Phone
					case "Hm Phone":
						row.Cells.Add(_patCur.HmPhone);
						if(_patCur.PreferContactMethod==ContactMethod.HmPhone || _patCur.PreferContactMethod==ContactMethod.None) {
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
						row.Cells.Add(_patNoteCur.ICEName);
						row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.FamilyModuleICE].ItemColor;
						break;
					#endregion ICE Name
					#region ICE Phone
					case "ICE Phone":
						row.Cells.Add(_patNoteCur.ICEPhone);
						row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.FamilyModuleICE].ItemColor;
						break;
					#endregion ICE Phone
					#region Language
					case "Language":
						if(_patCur.Language=="" || _patCur.Language==null) {
							row.Cells.Add("");
						}
						else {
							try {
								row.Cells.Add(CodeBase.MiscUtils.GetCultureFromThreeLetter(_patCur.Language).DisplayName);
								//row.Cells.Add(CultureInfo.GetCultureInfo(PatCur.Language).DisplayName);
							}
							catch {
								row.Cells.Add(_patCur.Language);
							}
						}
						break;
					#endregion Language
					#region Last
					case "Last":
						row.Cells.Add(_patCur.LName);
						break;
					#endregion Last
					#region Middle
					case "Middle":
						row.Cells.Add(_patCur.MiddleI);
						break;
					#endregion Middle
					#region PatFields
					case "PatFields":
						PatFieldL.AddPatFieldsToGrid(_gridPat,_arrayPatFields.ToList(),FieldLocations.Family);
						break;
					#endregion PatFields
					#region Pat Restrictions
					case "Pat Restrictions":
						List<PatRestriction> listPatRestricts=_loadData.ListPatRestricts??PatRestrictions.GetAllForPat(_patCur.PatNum);
						if(listPatRestricts.Count==0) {
							row.Cells.Add(Lan.g("TablePatient","None"));//row added outside of switch statement
						}
						for(int i=0;i<listPatRestricts.Count;i++) {
							row=new GridRow();
							if(string.IsNullOrWhiteSpace(displayFieldCur.Description)) {
								row.Cells.Add(displayFieldCur.InternalName);
							}
							else {
								row.Cells.Add(displayFieldCur.Description);
							}
							row.Cells.Add(PatRestrictions.GetPatRestrictDesc(listPatRestricts[i].PatRestrictType));
							row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.FamilyModPatRestrict].ItemColor;//Patient Restrictions (hard coded in convertdatabase4)
							if(i==listPatRestricts.Count-1) {//last row added outside of switch statement
								break;
							}
							_gridPat.ListGridRows.Add(row);
						}
						break;
					#endregion Pat Restrictions
					#region Payor Types
					case "Payor Types":
						row.Tag="Payor Types";
						row.Cells.Add(_loadData.PayorTypeDesc??PayorTypes.GetCurrentDescription(_patCur.PatNum));
						break;
					#endregion Payor Types
					#region Position
					case "Position":
						row.Cells.Add(_patCur.Position.ToString());
						break;
					#endregion Position
					#region Preferred
					case "Preferred":
						row.Cells.Add(_patCur.Preferred);
						break;
					#endregion Preferred
					#region Preferred Pronoun
					case "Preferred Pronoun":
						row.Cells.Add(_patNoteCur.Pronoun.ToString());
						break;
					#endregion
					#region Primary Provider
					case "Primary Provider":
						if(_patCur.PriProv!=0) {
							row.Cells.Add(Providers.GetLongDesc(Patients.GetProvNum(_patCur)));
						}
						else {
							row.Cells.Add(Lan.g("TablePatient","None"));
						}
						break;
					#endregion Primary Provider
					#region References
					case "References":
						List<CustRefEntry> custREList=_loadData.ListCustRefEntries??CustRefEntries.GetEntryListForCustomer(_patCur.PatNum);
						if(custREList.Count==0) {
							row.Cells.Add(Lan.g("TablePatient","None"));
							row.Tag="References";
							row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.FamilyModuleReferral].ItemColor;
						}
						else {
							row.Cells.Add(Lan.g("TablePatient",""));
							row.Tag="References";
							row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.FamilyModuleReferral].ItemColor;
							_gridPat.ListGridRows.Add(row);
						}
						for(int i=0;i<custREList.Count;i++) {
							row=new GridRow();
							if(custREList[i].PatNumRef==_patCur.PatNum) {
								row.Cells.Add(custREList[i].DateEntry.ToShortDateString());
								row.Cells.Add("For: "+CustReferences.GetCustNameFL(custREList[i].PatNumCust));
							}
							else {
								row.Cells.Add("");
								row.Cells.Add(CustReferences.GetCustNameFL(custREList[i].PatNumRef));
							}
							row.Tag=custREList[i];
							row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.FamilyModuleReferral].ItemColor;
							if(i<custREList.Count-1) {
								_gridPat.ListGridRows.Add(row);
							}
						}
						break;
					#endregion References
					#region Referrals
					case "Referrals":
						List<RefAttach> listRefs=_loadData.ListRefAttaches??RefAttaches.Refresh(_patCur.PatNum);
						List<RefAttach> listRefsFiltered= new List<RefAttach>();
						listRefsFiltered.AddRange(listRefs.Where(x => x.RefType==ReferralType.RefCustom).DistinctBy(x => x.ReferralNum).ToList());
						listRefsFiltered.AddRange(listRefs.Where(x => x.RefType==ReferralType.RefFrom).DistinctBy(x => x.ReferralNum).ToList());
						listRefsFiltered.AddRange(listRefs.Where(x => x.RefType==ReferralType.RefTo).DistinctBy(x => x.ReferralNum).ToList());
						listRefs=listRefsFiltered;
						if(listRefs.Count==0){
							row.Cells.Add(Lan.g("TablePatient","None"));
							row.Tag="Referral";
							row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.FamilyModuleReferral].ItemColor;
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
								if(!string.IsNullOrWhiteSpace(displayFieldCur.Description)) {
									row.Cells.Add(displayFieldCur.Description);
								}
								else {
									row.Cells.Add(Lan.g("TablePatient","Referral"));
								}
							}
							try {
								Referral referral=Referrals.GetFromList(listRefs[i].ReferralNum);
								string refInfo=Referrals.GetNameLF(listRefs[i].ReferralNum);
								string phoneInfo=Referrals.GetPhone(listRefs[i].ReferralNum);
								if(!string.IsNullOrWhiteSpace(phoneInfo)) {
									refInfo+=$"\r\n{phoneInfo}";
								}
								if(!string.IsNullOrWhiteSpace(referral.DisplayNote)) {
									refInfo+=$"\r\n{Lan.g("Referral","Display Note")}: {referral.DisplayNote}";
								}
								if(!string.IsNullOrWhiteSpace(listRefs[i].Note)) {
									refInfo+=$"\r\n{Lan.g("RefAttach","Patient Note")}: {listRefs[i].Note}";
								}
								row.Cells.Add(refInfo);
							}
							catch {
								row.Cells.Add("");//if referral is null because using random keys and had bug.
							}
							row.Tag="Referral";
							row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.FamilyModuleReferral].ItemColor;
							if(i<listRefs.Count-1) {
								_gridPat.ListGridRows.Add(row);
							}
						}
						break;
					#endregion Referrals
					#region ResponsParty
					case "ResponsParty":
						if(_patCur.ResponsParty==0) {
							row.Cells.Add("");
						}
						else {
							row.Cells.Add((_loadData.ResponsibleParty??Patients.GetLim(_patCur.ResponsParty)).GetNameLF());
						}
						row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.FamilyModuleReferral].ItemColor;
						break;
					#endregion ResponsParty
					#region Salutation
					case "Salutation":
						row.Cells.Add(_patCur.Salutation);
						break;
					#endregion Salutation
					#region Sec. Provider
					case "Sec. Provider":
						if(_patCur.SecProv != 0) {
							row.Cells.Add(Providers.GetLongDesc(_patCur.SecProv));
						}
						else {
							row.Cells.Add(Lan.g("TablePatient","None"));
						}
						break;
					#endregion Sec. Provider
					#region SS#
					case "SS#":
						if(PrefC.GetBool(PrefName.PatientSSNMasked)) {
							row.Cells.Add(Patients.SSNFormatHelper(_patCur.SSN,true));
							row.Tag="SS#";//Used later to tell if we're right clicking on the SSN row
						}
						else {
							row.Cells.Add(Patients.SSNFormatHelper(_patCur.SSN,false));
						}
						break;
					#endregion SS#
					#region State
					case "State":
						row.Cells.Add(_patCur.State);
						break;
					#endregion State
					#region Status
					case "Status":
						row.Cells.Add(_patCur.PatStatus.ToString());
						if(_patCur.PatStatus==PatientStatus.Deceased) {
							row.ColorText=Color.Red;
						}
						break;
					#endregion Status
					#region Super Head
					case "Super Head":
						string fieldVal="";
						if(_patCur.SuperFamily!=0) {
							Patient supHead=_loadData.SuperFamilyGuarantors.FirstOrDefault(x => x.PatNum==_patCur.SuperFamily)??Patients.GetPat(_patCur.SuperFamily);
							fieldVal=supHead.GetNameLF()+" ("+supHead.PatNum+")";
						}
						row.Cells.Add(fieldVal);
						break;
					#endregion Super Head
					#region Tax Address
					case "Tax Address":
						if (PrefC.IsODHQ) {
							row.Bold=true;
							Address address=Addresses.GetOneByPatNum(_patCur.PatNum);//can be null
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
						}
						break;
					#endregion Tax Address
					#region Title
					case "Title":
						row.Cells.Add(_patCur.Title);
						break;
					#endregion Title
					#region Ward
					case "Ward":
						row.Cells.Add(_patCur.Ward);
						break;
					#endregion Ward
					#region Wireless Ph
					case "Wireless Ph":
						row.Cells.Add(_patCur.WirelessPhone);
						if(_patCur.PreferContactMethod==ContactMethod.WirelessPh) {
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
						row.Cells.Add(_patCur.WkPhone);
						if(_patCur.PreferContactMethod==ContactMethod.WkPhone) {
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
						row.Cells.Add(_patCur.Zip);
						break;
					#endregion Zip
				}
				#endregion Value Column
				if(displayFieldCur.InternalName=="PatFields") {
					//don't add the row here
				}
				else {
					_gridPat.ListGridRows.Add(row);
				}
			}
			_gridPat.EndUpdate();
		}
		#endregion Methods - Private - GridPatient

		#region Methods - Private - GridFamily
		private void FillFamilyData() {
			_gridFamily.BeginUpdate();
			_gridFamily.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TablePatient","Name"),140);
			_gridFamily.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePatient","Position"),65);
			_gridFamily.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePatient","Gender"),55);
			_gridFamily.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePatient","Status"),65);
			_gridFamily.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePatient","Age"),45);
			_gridFamily.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePatient","Recall Due"),80);
			_gridFamily.Columns.Add(col);
			_gridFamily.ListGridRows.Clear();
			if(_patCur==null) {
				_gridFamily.EndUpdate();
				return;
			}
			GridRow row;
			DateTime dateRecall;
			GridCell cell;
			int selectedRow=-1;
			for(int i=0;i<_famCur.ListPats.Length;i++) {
				if(PatientLinks.WasPatientMerged(_famCur.ListPats[i].PatNum,_loadData.ListMergeLinks) && _famCur.ListPats[i].PatNum!=_patCur.PatNum) {
					//Hide merged patients so that new things don't get added to them. If the user really wants to find this patient, they will have to use 
					//the Select Patient window.
					continue;
				}
				row=new GridRow();
				row.Cells.Add(_famCur.GetNameInFamLFI(i));
				row.Cells.Add(Lan.g("enumPatientPosition",_famCur.ListPats[i].Position.ToString()));
				row.Cells.Add(Lan.g("enumPatientGender",_famCur.ListPats[i].Gender.ToString()));
				row.Cells.Add(Lan.g("enumPatientStatus",_famCur.ListPats[i].PatStatus.ToString()));
				row.Cells.Add(PatientLogic.DateToAgeString(_famCur.ListPats[i].Birthdate,_famCur.ListPats[i].DateTimeDeceased));
				dateRecall=DateTime.MinValue;
				for(int j=0;j<_listRecalls.Count;j++) {
					if(_listRecalls[j].PatNum==_famCur.ListPats[i].PatNum
						&& (_listRecalls[j].RecallTypeNum==PrefC.GetLong(PrefName.RecallTypeSpecialProphy)
						|| _listRecalls[j].RecallTypeNum==PrefC.GetLong(PrefName.RecallTypeSpecialPerio)))
					{
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
				row.Tag=_famCur.ListPats[i];
				_gridFamily.ListGridRows.Add(row);
				int idx=_gridFamily.ListGridRows.Count-1;
				if(_famCur.ListPats[i].PatNum==_patCur.PatNum) {
					selectedRow=idx;
				}
			}
			_gridFamily.EndUpdate();
			_gridFamily.SetSelected(selectedRow,true);
		}

		private void ToolButAdd_Click() {
			if(!Security.IsAuthorized(Permissions.PatientEdit)) {
				return;
			}
			//At HQ, some resellers don't add clients through the reseller portal.
			//Instead, they contact the conversions department and conversions creates a new account for them and adds them to the superfamily.
			//These accounts are acceptable to add because HQ understands they are not accounts designed to be managed by the Reseller Portal.
			Patient patTemp=new Patient();
			patTemp.LName         =_patCur.LName;
			patTemp.PatStatus     =PatientStatus.Deleted;
			patTemp.Gender        =PatientGender.Unknown;
			patTemp.Address       =_patCur.Address;
			patTemp.Address2      =_patCur.Address2;
			patTemp.City          =_patCur.City;
			patTemp.State         =_patCur.State;
			patTemp.Zip           =_patCur.Zip;
			patTemp.HmPhone       =_patCur.HmPhone;
			patTemp.WirelessPhone =_patCur.WirelessPhone;
			patTemp.WkPhone       =_patCur.WkPhone;
			patTemp.Email         =_patCur.Email;
			patTemp.TxtMsgOk      =_patCur.TxtMsgOk;
			patTemp.ShortCodeOptIn=_patCur.ShortCodeOptIn;
			patTemp.Guarantor     =_patCur.Guarantor;
			patTemp.CreditType    =_patCur.CreditType;
			if(!PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
				patTemp.PriProv     =_patCur.PriProv;
			}
			patTemp.SecProv       =_patCur.SecProv;
			patTemp.FeeSched      =_patCur.FeeSched;
			patTemp.BillingType   =_patCur.BillingType;
			patTemp.AddrNote      =_patCur.AddrNote;
			patTemp.ClinicNum     =_patCur.ClinicNum;//this is probably better in case they don't have user.ClinicNums set.
			//tempPat.ClinicNum  =Security.CurUser.ClinicNum;
			if(Patients.GetPat(patTemp.Guarantor).SuperFamily!=0) {
				patTemp.SuperFamily=_patCur.SuperFamily;
			}
			Patients.Insert(patTemp,false);
			SecurityLogs.MakeLogEntry(Permissions.PatientCreate,patTemp.PatNum,"Created from Family Module Add button.");
			CustReference custReference=new CustReference();
			custReference.PatNum=patTemp.PatNum;
			CustReferences.Insert(custReference);
			//add the tempPat to the FamCur list, but ModuleSelected below will refill the FamCur list in case the user cancels and tempPat is deleted
			//This would be a faster way to add to the array, but since it is not a pattern that is used anywhere we will use the alternate method of
			//creating a list, adding the patient, and converting back to an array
			//Array.Resize(ref FamCur.ListPats,FamCur.ListPats.Length+1);
			//FamCur.ListPats[FamCur.ListPats.Length-1]=tempPat;
			//Adding the temp patient to the FamCur.ListPats without calling GetFamily which makes a call to the db
			List<Patient> listPatientsTemp=_famCur.ListPats.ToList();
			listPatientsTemp.Add(patTemp);
			_famCur.ListPats=listPatientsTemp.ToArray();
			using FormPatientEdit formPatientEdit=new FormPatientEdit(patTemp,_famCur);
			formPatientEdit.IsNew=true;
			formPatientEdit.ShowDialog();
			if(formPatientEdit.DialogResult==DialogResult.OK) {
				FormOpenDental.S_Contr_PatientSelected(patTemp,false);
				ModuleSelected(patTemp.PatNum);
			}
			else {
				ModuleSelected(_patCur.PatNum);
			}
		}

		private void ToolButDelete_Click() {
			if(!Security.IsAuthorized(Permissions.PatientEdit)) {
				return;
			}
			//this doesn't actually delete the patient, just changes their status
			//and they will never show again in the patient selection list.
			//check for plans, appointments, procedures, etc.
			List<Procedure> listProcs=Procedures.Refresh(_patCur.PatNum);
			List<Appointment> listAppts=Appointments.GetPatientData(_patCur.PatNum);
			List<Claim> listClaims=Claims.Refresh(_patCur.PatNum);
			Adjustment[] arrayAdjustments=Adjustments.Refresh(_patCur.PatNum);
			PaySplit[] arrayPaySplits=PaySplits.Refresh(_patCur.PatNum);//
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(_patCur.PatNum);
			List<Commlog> listCommlogs=Commlogs.Refresh(_patCur.PatNum);
			int countPayPlans=PayPlans.GetDependencyCount(_patCur.PatNum);
			List<InsSub> listInsSubs=InsSubs.RefreshForFam(_famCur);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			List<MedicationPat> listMedicationPats=MedicationPats.Refresh(_patCur.PatNum,false);
			_listPatPlans=PatPlans.Refresh(_patCur.PatNum);
			//CovPats.Refresh(planList,PatPlanList);
			List<RefAttach> listRefAttaches=RefAttaches.Refresh(_patCur.PatNum);
			List<Sheet> listSheets=Sheets.GetForPatient(_patCur.PatNum);
			RepeatCharge[] arrayRepeatCharges=RepeatCharges.Refresh(_patCur.PatNum);
			List<CreditCard> listCreditCards=CreditCards.Refresh(_patCur.PatNum);
			RegistrationKey[] arrayRegistrationKeys=RegistrationKeys.GetForPatient(_patCur.PatNum);
			List<long> listPatNumClones=Patients.GetClonePatNumsAll(_patCur.PatNum);
			long discountPlanNum=DiscountPlanSubs.GetDiscountPlanNumForPat(_patCur.PatNum);
			bool hasProcs=listProcs.Count>0;
			bool hasAppt=listAppts.Count>0;
			bool hasClaims=listClaims.Count>0;
			bool hasAdj=arrayAdjustments.Length>0;
			bool hasPay=arrayPaySplits.Length>0;
			bool hasClaimProcs=listClaimProcs.Count>0;
			bool hasComm=listCommlogs.Count>0;
			bool hasPayPlans=countPayPlans>0;
			//Has patplan or is a subscriber to an insplan
			bool hasInsPlans=_listPatPlans.Count>0 || listInsSubs.Any(x => x.Subscriber==_patCur.PatNum);
			bool hasMeds=listMedicationPats.Count>0;
			bool isSuperFamilyHead=_patCur.PatNum==_patCur.SuperFamily;
			bool hasRef=listRefAttaches.Count>0;
			bool hasSheets=listSheets.Count>0;
			bool hasRepeat=arrayRepeatCharges.Length>0;
			bool hasCC=listCreditCards.Count>0;
			bool hasRegKey=arrayRegistrationKeys.Length>0;
			bool hasPerio=PerioExams.GetExamsTable(_patCur.PatNum).Rows.Count>0;
			bool hasClones=(listPatNumClones.Count>1);//The list of "clones for all" will always include the current patient.
			bool hasDiscount=discountPlanNum>0;
			bool hasAllergies=Allergies.GetAll(_patCur.PatNum,true).Any();
			bool hasProblems=Diseases.Refresh(true,_patCur.PatNum).Any();
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
			Patient patOld=_patCur.Copy();
			if(_patCur.PatNum==_patCur.Guarantor) {//if selecting guarantor
				if(_famCur.ListPats.Length==1) {
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Patient?")) {
						return;
					}
					_patCur.PatStatus=PatientStatus.Deleted;
					_patCur.ChartNumber="";
					_patCur.ClinicNum=0;
					_patCur.FeeSched=0;
					Popups.MoveForDeletePat(_patCur);
					_patCur.SuperFamily=0;
					Patients.Update(_patCur,patOld);
					for(int i=0;i<_listRecalls.Count;i++) {
						if(_listRecalls[i].PatNum==_patCur.PatNum) {
							_listRecalls[i].IsDisabled=true;
							_listRecalls[i].DateDue=DateTime.MinValue;
							Recalls.Update(_listRecalls[i]);
						}
					}
					SecurityLogs.MakeLogEntry(Permissions.PatientEdit,patOld.PatNum,"Patient deleted");
					FormOpenDental.S_Contr_PatientSelected(new Patient(),false);
					ModuleSelected(0);
					//does not delete notes or plans, etc.
				}
				else {
					MessageBox.Show(Lan.g(this,"You cannot delete the guarantor if there are other family members. You would have to make a different family member the guarantor first."));
				}
			}
			else {//not selecting guarantor
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Patient?")) {
					return;
				}
				_patCur.PatStatus=PatientStatus.Deleted;
				_patCur.ChartNumber="";
				_patCur.ClinicNum=0;
				_patCur.FeeSched=0;
				Popups.MoveForDeletePat(_patCur);
				_patCur.Guarantor=_patCur.PatNum;
				_patCur.SuperFamily=0;
				Patients.Update(_patCur,patOld);
				for(int i=0;i<_listRecalls.Count;i++) {
					if(_listRecalls[i].PatNum==_patCur.PatNum) {
						_listRecalls[i].IsDisabled=true;
						_listRecalls[i].DateDue=DateTime.MinValue;
						Recalls.Update(_listRecalls[i]);
					}
				}
				SecurityLogs.MakeLogEntry(Permissions.PatientEdit,patOld.PatNum,"Patient deleted");
				ModuleSelected(patOld.Guarantor);//Sets PatCur to PatOld guarantor.
				FormOpenDental.S_Contr_PatientSelected(_patCur,false);//PatCur is now the Guarantor.
			}
			PatientL.RemoveFromMenu(patOld.PatNum);//Always remove deleted patients from the dropdown menu.
		}

		private void ToolButGuarantor_Click() {
			if(_patCur.PatNum==_patCur.Guarantor) {
				MessageBox.Show(Lan.g(this,"Patient is already the guarantor.  Please select a different family member."));
				return;
			}
			if(MessageBox.Show(Lan.g(this,"Make the selected patient the guarantor?")
				,"",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
				return;
			}
			if(_patCur.SuperFamily==_patCur.Guarantor) {//guarantor is also the head of a super family
				Patients.MoveSuperFamily(_patCur.SuperFamily,_patCur.PatNum);
			}
			Patients.ChangeGuarantorToCur(_famCur,_patCur);
			ModuleSelected(_patCur.PatNum);
		}

		private void ToolButMove_Click() {
			Patient patOld=_patCur.Copy();
			//Patient PatCur;
			if(_patCur.PatNum==_patCur.Guarantor) {//if guarantor selected
				if(_patCur.SuperFamily==_patCur.Guarantor && _loadData.SuperFamilyMembers.Count>1) {
					MsgBox.Show(this,"You cannot move the head of a super family. If you wish to move the super family head, you must first remove all other super family members.");
					return;
				}
				if(_famCur.ListPats.Length==1) {//and no other family members
					if(!MovePats(patOld)) {
						return;
					}
				}
				else {//there are other family members
					foreach(Patient pat in _famCur.ListPats) {
						if(pat.PatNum==_patCur.PatNum) {
							continue;
						}
						List<PatientLink> listPatLinks=PatientLinks.GetLinks(pat.PatNum,PatientLinkType.Merge);//If there is another family member, make sure it is merged.  
						if(listPatLinks.Count==0 || !listPatLinks.Exists(x => x.PatNumFrom==pat.PatNum)) {//If it's not merged, user can't move guarantor.
							MessageBox.Show(Lan.g(this,"You cannot move the guarantor.  If you wish to move the guarantor, you must make another family member the guarantor first."));
							return;
						}
					}
					if(!MovePats(patOld,_famCur)) {
						return;
					}
				}
			}
			else {//guarantor not selected
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
						Popups.CopyForMovingFamilyMember(_patCur);//Copy Family Level Popups to new family. 
						//Don't need to copy SuperFamily Popups. Stays in same super family.
						_patCur.Guarantor=_patCur.PatNum;
						//keep current superfamily
						Patients.Update(_patCur,patOld);
						//if moving a superfamily non-guar family member out as guar of their own family within the sf, and pref is set, add ins to family members if necessary
						if(_patCur.SuperFamily>0 && PrefC.GetBool(PrefName.SuperFamNewPatAddIns)) {
							AddSuperGuarPriInsToFam(_patCur.Guarantor);
						}
						SecurityLogs.MakeLogEntry(Permissions.PatientEdit,_patCur.PatNum,"Patient moved to new family.");
						break;
					case DialogResult.No://move to an existing family
						if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Select the family to move this patient to from the list that will come up next.")) {
							return;
						}
						using(FormPatientSelect formPatSelect=new FormPatientSelect()) {
							formPatSelect.SelectionModeOnly=true;
							formPatSelect.ShowDialog();
							if(formPatSelect.DialogResult!=DialogResult.OK) {
								return;
							}
							Patient patInNewFam=Patients.GetPat(formPatSelect.SelectedPatNum);
							if(patInNewFam.Guarantor==_patCur.Guarantor) {
								return;// Patient is already a part of the family.
							}
							Popups.CopyForMovingFamilyMember(_patCur);//Copy Family Level Popups to new Family. 
							if(_patCur.SuperFamily!=patInNewFam.SuperFamily) {//If they are moving into or out of a superfamily
								if(_patCur.SuperFamily!=0) {//If they are currently in a SuperFamily.  Otherwise, no superfamily popups to worry about.
									Popups.CopyForMovingSuperFamily(_patCur,patInNewFam.SuperFamily);
								}
							}
							_patCur.Guarantor=patInNewFam.Guarantor;
							_patCur.SuperFamily=patInNewFam.SuperFamily;//assign to the new superfamily
						}
						Patients.Update(_patCur,patOld);
						SecurityLogs.MakeLogEntry(Permissions.PatientEdit,_patCur.PatNum,"Patient moved from family of '"+patOld.Guarantor+"' "
							+"to existing family of '"+_patCur.Guarantor+"'");
						break;
				}
			}//end guarantor not selected
			ModuleSelected(_patCur.PatNum);
		}
		#endregion Methods - Private - GridFamily

		#region Methods - Private - GridRecall
		private void FillGridRecall() {
			//We never show a horizScroll or wrap because there's simply not enough space.  They can double click if they want more info.
			//The window that comes up on the double click could use a bit of help.
			_gridRecall.BeginUpdate();
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
			int widthAvail=Width-_gridRecall.Left;
			LayoutManager.MoveWidth(_gridRecall,widthAvail);
			/*if(LayoutManager.Scale(width)>widthAvail){
				gridRecall.HScrollVisible=true;
			}
			else{
				gridRecall.HScrollVisible=false;
			}*/
			_gridRecall.Columns.Clear();
			GridColumn col;
			for(int i=0;i<listDisplayFieldsRecall.Count;i++) {
				if(listDisplayFieldsRecall[i].Description=="") {
					col=new GridColumn(listDisplayFieldsRecall[i].InternalName,listDisplayFieldsRecall[i].ColumnWidth);
				}
				else {
					col=new GridColumn(listDisplayFieldsRecall[i].Description,listDisplayFieldsRecall[i].ColumnWidth);
				}
				_gridRecall.Columns.Add(col);
			}
			_gridRecall.ListGridRows.Clear();
			if(_patCur==null) {
				_gridRecall.EndUpdate();
				return;
			}
			//we just want the recall for the current patient
			List<Recall> listRecallsPat=new List<Recall>();
			for(int i=0;i<_listRecalls.Count;i++) {
				if(_listRecalls[i].PatNum==_patCur.PatNum) {
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
							}
							else {
								cell=new GridCell(listRecallsPat[i].DateDue.ToShortDateString());
								if(listRecallsPat[i].DateDue<DateTime.Today) {
									cell.Bold=YN.Yes;
									cell.ColorText=Color.Firebrick;
								}
								row.Cells.Add(cell);
							}
							break;
						case "Sched Date":
							if(listRecallsPat[i].DateScheduled.Year<1880) {
								row.Cells.Add("");
							}
							else {
								row.Cells.Add(listRecallsPat[i].DateScheduled.ToShortDateString());
							}
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
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Interval":
							row.Cells.Add(listRecallsPat[i].RecallInterval.ToString());
							break;
					}
				}
				_gridRecall.ListGridRows.Add(row);
			}
			_gridRecall.EndUpdate();
		}
		#endregion Methods - Private - GridRecall

		#region Methods - Private - GridSuperFam
		///<summary>Adds the super family guarantor's primary insurance plan to each family member in Fam.  Each family member will be their own
		///subscriber with SubscriberID set to the patient's MedicaidID if one has been entered for the patient.  If a family member does not have a 
		///MedicaidID entered, FormInsPlan will open and prompt the user to enter a SubscriberID.</summary>
		private void AddSuperGuarPriInsToFam(long guarNum) {
			Patient patSuperFamGuar=Patients.GetPat(_patCur.SuperFamily);
			if(patSuperFamGuar==null) {//should never happen, but just in case
				return;
			}
			List<InsSub> listInsSubsSuper=InsSubs.GetListForSubscriber(patSuperFamGuar.PatNum);
			if(listInsSubsSuper.Count==0) {//super family guar is not the subscriber for any insplans
				return;
			}
			List<PatPlan> listPatPlansSuper=PatPlans.Refresh(patSuperFamGuar.PatNum);
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
			InsSub insSubCur;
			List<PatPlan> listPatPlansForPat;
			Family famCur=Patients.GetFamily(guarNum);
			List<InsSub> listInsSubsForFam=InsSubs.RefreshForFam(famCur);
			List<InsPlan> listInsPlansForFam=InsPlans.RefreshForSubList(listInsSubsForFam);
			bool hasPatPlanAdded=false;
			foreach(Patient pat in famCur.ListPats) {//possibly filter by PatStatus, i.e. .Where(x => x.PatStatus==PatientStatus.Patient)
				listPatPlansForPat=PatPlans.Refresh(pat.PatNum);
				insSubCur=listInsSubsForFam.FirstOrDefault(x => x.Subscriber==pat.PatNum && x.PlanNum==insSub.PlanNum);
				if(insSubCur!=null) {//InsSub already exists for this Patient and InsPlan
					if(listPatPlansForPat.Any(x => x.InsSubNum==insSubCur.InsSubNum)) {//PatPlan exists for this Patient and InsSub, nothing to do
						continue;
					}
				}
				else {//insSubCur==null, no InsSub exists for this patient and plan, insert new one
					insSubCur=new InsSub();
					insSubCur.PlanNum=insSub.PlanNum;
					insSubCur.Subscriber=pat.PatNum;
					insSubCur.ReleaseInfo=insSub.ReleaseInfo;
					insSubCur.AssignBen=insSub.AssignBen;
					//insSubNew.BenefitNotes=sub.BenefitNotes;//not the BenefitNotes, since these could be specific to a patient
					insSubCur.SubscriberID=string.IsNullOrWhiteSpace(pat.MedicaidID)?"":pat.MedicaidID;
					//insSubNew.SubscNote=sub.SubscNote;//not the subscriber note, since every patient in super family is their own subscriber to this plan
					insSubCur.InsSubNum=InsSubs.Insert(insSubCur);
					listInsSubsForFam.Add(insSubCur.Copy());
				}
				patPlanNew=new PatPlan();
				patPlanNew.Ordinal=(byte)(listPatPlansForPat.Count+1);//so the ordinal of the first entry will be 1, NOT 0.
				patPlanNew.PatNum=pat.PatNum;
				patPlanNew.InsSubNum=insSubCur.InsSubNum;
				patPlanNew.Relationship=Relat.Self;
				patPlanNew.PatPlanNum=PatPlans.Insert(patPlanNew);
				listPatPlansForPat.Add(patPlanNew.Copy());
				if(string.IsNullOrWhiteSpace(insSubCur.SubscriberID)) {
					MessageBox.Show(this,Lan.g(this,"Enter the SubscriberID for")+" "+pat.GetNameFL()+".");
					using FormInsPlan formInsPlan=new FormInsPlan(InsPlans.GetPlan(insSubCur.PlanNum,listInsPlansForFam),patPlanNew,insSubCur);
					formInsPlan.IsNewPlan=false;
					formInsPlan.IsNewPatPlan=true;
					formInsPlan.ShowDialog();//this updates estimates. If cancel, then patplan is deleted. If cancel and planIsNew, then plan and benefits are deleted
					if(formInsPlan.DialogResult!=DialogResult.OK) {
						continue;
					}
				}
				else {
					//compute estimates with new insurance plan
					List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
					List<Procedure> listProcs=Procedures.Refresh(pat.PatNum);
					List<Benefit> listBenefits=Benefits.Refresh(listPatPlansForPat,listInsSubsForFam);
					Procedures.ComputeEstimatesForAll(pat.PatNum,listClaimProcs,listProcs,listInsPlansForFam,listPatPlansForPat,listBenefits,pat.Age,listInsSubsForFam);
				}
				hasPatPlanAdded=true;
				if(pat.HasIns!="I") {
					Patient patOld=pat.Copy();
					pat.HasIns="I";
					Patients.Update(pat,patOld);
				}
				Appointments.UpdateInsPlansForPat(pat.PatNum);
			}
			if(hasPatPlanAdded) {
				SecurityLogs.MakeLogEntry(Permissions.PatPlanCreate,patSuperFamGuar.PatNum,"Inserted new PatPlans for each family member of the super family guarantor.");
			}
		}

		private void FillGridSuperFam() {
			_gridSuperFam.BeginUpdate();
			_gridSuperFam.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("gridSuperFam","Name"),280);
			_gridSuperFam.Columns.Add(col);
			col=new GridColumn(Lan.g("gridSuperFam","Stmt"),280){ IsWidthDynamic=true };
			_gridSuperFam.Columns.Add(col);
			_gridSuperFam.ListGridRows.Clear();
			if(_patCur==null) {
				_gridSuperFam.EndUpdate();
				return;
			}
			GridRow row;
			_listPatSuperFamilyGuarantors.Sort(sortPatientListBySuperFamily);
			_listPatSuperFamilyMembers.Sort(sortPatientListBySuperFamily);
			string strSuperFam="";
			for(int i=0;i<_listPatSuperFamilyGuarantors.Count;i++) {
				row=new GridRow();
				strSuperFam=_listPatSuperFamilyGuarantors[i].GetNameLF();
				for(int j=0;j<_listPatSuperFamilyMembers.Count;j++) {
					if(PatientLinks.WasPatientMerged(_listPatSuperFamilyMembers[j].PatNum,_loadData.ListMergeLinks) && _listPatSuperFamilyMembers[j].PatNum!=_patCur.PatNum) {
						//Hide merged patients so that new things don't get added to them. If the user really wants to find this patient, they will have to use 
						//the Select Patient window.
						continue;
					}
					if(_listPatSuperFamilyMembers[j].Guarantor==_listPatSuperFamilyGuarantors[i].Guarantor && _listPatSuperFamilyMembers[j].PatNum!=_listPatSuperFamilyGuarantors[i].PatNum) {
						strSuperFam+="\r\n   "+StringTools.Truncate(_listPatSuperFamilyMembers[j].GetNameLF(),40,true);
					}
				}
				row.Cells.Add(strSuperFam);
				row.Tag=_listPatSuperFamilyGuarantors[i].PatNum;
				if(i==0) {
					row.Cells[0].Bold=YN.Yes;
					row.Cells[0].ColorText=Color.OrangeRed;
				}
				if(_listPatSuperFamilyGuarantors[i].HasSuperBilling) {
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}
				_gridSuperFam.ListGridRows.Add(row);
			}
			_gridSuperFam.EndUpdate();
			for(int i=0;i<_gridSuperFam.ListGridRows.Count;i++) {
				if((long)_gridSuperFam.ListGridRows[i].Tag==_patCur.Guarantor) {
					_gridSuperFam.SetSelected(i,true);
					break;
				}
			}
		}

		private int sortPatientListBySuperFamily(Patient pat1,Patient pat2) {
			if(pat1.PatNum==pat2.PatNum) {
				return 0;
			}
			if(pat1.PatNum==pat1.SuperFamily) {//Superheads always go to the top no matter what.
				return -1;
			}
			if(pat2.PatNum==pat2.SuperFamily) {
				return 1;
			}
			switch(_sortStratSuperFam) {
				case SortStrategy.NameAsc:
					return pat1.GetNameLF().CompareTo(pat2.GetNameLF());
				case SortStrategy.NameDesc:
					return pat2.GetNameLF().CompareTo(pat1.GetNameLF());
				case SortStrategy.PatNumAsc:
					return pat1.PatNum.CompareTo(pat2.PatNum);
				case SortStrategy.PatNumDesc:
					return pat2.PatNum.CompareTo(pat1.PatNum);
				default:
					return pat1.PatNum.CompareTo(pat2.PatNum);//Default behavior
			}
		}

		private void ToolButAddSuper_Click() {
			//At HQ, some resellers don't add clients through the reseller portal.
			//Instead, they contact the conversions department and conversions creates a new account for them and adds them to the superfamily.
			//These accounts are acceptable to add because HQ understands they are not accounts designed to be managed by the Reseller Portal.
			if(_patCur.SuperFamily==0) {
				Patients.AssignToSuperfamily(_patCur.Guarantor,_patCur.Guarantor);
				SecurityLogs.MakeLogEntry(
					Permissions.PatientEdit,
					_patCur.PatNum,
					Lan.g(this,"Patient added to superfamily. Previous superfamily guarantor PatNum:0, and current superfamily guarantor PatNum:") +_patCur.SuperFamily + "."
					
					);
				ModuleSelected(_patCur.PatNum);
				return;
			}
			//we must want to add some other family to this superfamily
			using FormPatientSelect formPatSelect=new FormPatientSelect();
			formPatSelect.SelectionModeOnly=true;
			if(formPatSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			Patient patSelected=Patients.GetPat(formPatSelect.SelectedPatNum);
			long superFamilyGuarantorNumPrevious=patSelected.SuperFamily;
			if(patSelected.SuperFamily==_patCur.SuperFamily) {
				MsgBox.Show(this,"That patient is already part of this superfamily.");
				return;
			}
			List<Patient> listSuperFamPats=new List<Patient>();
			if(patSelected.SuperFamily==patSelected.Guarantor) {//selected patient's guarantor is the super family head of another super family
				listSuperFamPats=Patients.GetBySuperFamily(patSelected.SuperFamily);
			}
			DialogResult diagResult=DialogResult.None;
			if(listSuperFamPats.Any(x => x.Guarantor!=x.SuperFamily)) {//super family consists of more than one family
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
				Patients.MoveSuperFamily(patSelected.SuperFamily,_patCur.SuperFamily);
				if(PrefC.GetBool(PrefName.SuperFamNewPatAddIns)) {
					listSuperFamPats.Select(x => x.Guarantor).Distinct().ForEach(x => AddSuperGuarPriInsToFam(x));
				}
			}
			else if(diagResult.In(DialogResult.None,DialogResult.No)) {//None = the fam doesn't belong to another super fam, just move into this super fam
				if(diagResult==DialogResult.No) {
					Patients.DisbandSuperFamily(patSelected.SuperFamily);//adding to this super family will happen below
				}
				Patients.AssignToSuperfamily(patSelected.Guarantor,_patCur.SuperFamily);
				SecurityLogs.MakeLogEntry(
					Permissions.PatientEdit,
					patSelected.PatNum,
					Lan.g(this,"Patient added to superfamily. Previous superfamily guarantor PatNum:")+superFamilyGuarantorNumPrevious+
					Lan.g(this, " and current superfamily guarantor PatNum:") +_patCur.SuperFamily + "."
					);
				if(PrefC.GetBool(PrefName.SuperFamNewPatAddIns)) {
					AddSuperGuarPriInsToFam(patSelected.Guarantor);
				}
			}
			ModuleSelected(_patCur.PatNum);
		}

		private void ToolButDisbandSuper_Click() {
			if(_patCur.SuperFamily==0) {
				return;
			}
			Patient patSuperHead=Patients.GetPat(_patCur.SuperFamily);
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Would you like to disband and remove all members in the super family of "+patSuperHead.GetNameFL()+"?")) {
				return;
			}
			Popups.RemoveForDisbandingSuperFamily(_patCur);
			List<long> listPatientNumsInSuperFam=Patients.GetAllFamilyPatNumsForSuperFam(new List<long>(){_patCur.SuperFamily});
			for(int i=0;i<listPatientNumsInSuperFam.Count;i++) {
				SecurityLogs.MakeLogEntry(
					Permissions.PatientEdit,
					listPatientNumsInSuperFam[i],
					Lan.g(this,"Patient removed from superfamily. Previous superfamily guarantor PatNum:")+_patCur.SuperFamily+
					Lan.g(this,". Superfamily Disbanded.")
					);
			}
			Patients.DisbandSuperFamily(patSuperHead.PatNum);
			ModuleSelected(_patCur.PatNum);
		}

		private void ToolButRemoveSuper_Click() {
			long superFamilyGuarantorNumPrevious=_patCur.SuperFamily;
			if(_patCur.SuperFamily==_patCur.Guarantor) {
				MsgBox.Show(this,"You cannot delete the head of a super family.");
				return;
			}
			if(_patCur.SuperFamily==0) {
				return;
			}
			for(int i=0;i<_famCur.ListPats.Length;i++) {//remove whole family
				Patient patTemp=_famCur.ListPats[i].Copy();
				Popups.CopyForMovingSuperFamily(patTemp,0);
				patTemp.SuperFamily=0;
				Patients.Update(patTemp,_famCur.ListPats[i]);
				SecurityLogs.MakeLogEntry(
					Permissions.PatientEdit,
					patTemp.PatNum,
					Lan.g(this,"Patient removed from superfamily. Previous superfamily guarantor PatNum:")+superFamilyGuarantorNumPrevious+"."
					);
			}
			ModuleSelected(_patCur.PatNum);
		}
		#endregion Methods - Private - GridSuperFam

		#region Methods - Private - GridIns
		private void FillInsData() {
			if(_patCur!=null && _discountPlanSubCur!=null) {
				_gridIns.BeginUpdate();
				_gridIns.Title=Lan.g(this,"Discount Plan");
				_gridIns.Columns.Clear();
				_gridIns.ListGridRows.Clear();
				_gridIns.Columns.Add(new GridColumn("",170));
				_gridIns.Columns.Add(new GridColumn(Lan.g(this,"Discount Plan"),170));
				DiscountPlan discountPlan;
				if(_loadData.DiscountPlan==null || _loadData.DiscountPlan.DiscountPlanNum!=_discountPlanSubCur.DiscountPlanNum) {
					discountPlan=DiscountPlans.GetPlan(_discountPlanSubCur.DiscountPlanNum);
				}
				else {
					discountPlan=_loadData.DiscountPlan;
				}
				Def defAdjType=Defs.GetDef(DefCat.AdjTypes,discountPlan.DefNum);
				GridRow rowDiscount=new GridRow();
				rowDiscount.Cells.Add(Lan.g("TableDiscountPlans","Description"));
				rowDiscount.Cells.Add(discountPlan.Description);
				rowDiscount.ColorBackG=Defs.GetFirstForCategory(DefCat.MiscColors).ItemColor;
				_gridIns.ListGridRows.Add(rowDiscount);
				rowDiscount=new GridRow();
				rowDiscount.Cells.Add(Lan.g("TableDiscountPlans","Adjustment Type"));
				rowDiscount.Cells.Add(defAdjType.ItemName);
				_gridIns.ListGridRows.Add(rowDiscount);
				rowDiscount=new GridRow();
				rowDiscount.Cells.Add(Lan.g("TableDiscountPlans","Fee Schedule"));
				rowDiscount.Cells.Add(FeeScheds.GetDescription(discountPlan.FeeSchedNum));
				_gridIns.ListGridRows.Add(rowDiscount);
				rowDiscount=new GridRow();
				rowDiscount.Cells.Add(Lan.g("TableDiscountPlans","Date Effective"));
				if(_discountPlanSubCur.DateEffective.Year < 1880) {
					rowDiscount.Cells.Add("");
				}
				else {
					rowDiscount.Cells.Add(_discountPlanSubCur.DateEffective.ToShortDateString());
				}
				string discountPlanAnnualMax=discountPlan.AnnualMax.ToString();
				if(discountPlan.AnnualMax==-1) {
					discountPlanAnnualMax="";
				}
				_gridIns.ListGridRows.Add(new GridRow(Lan.g("TableDiscountPlans","Annual Max"),discountPlanAnnualMax));
				if(discountPlan.ExamFreqLimit>=0) {
					_gridIns.ListGridRows.Add(new GridRow(Lan.g("TableDiscountPlans","Exam Frequency"),discountPlan.ExamFreqLimit.ToString()));
				}
				if(discountPlan.ProphyFreqLimit>=0) {
					_gridIns.ListGridRows.Add(new GridRow(Lan.g("TableDiscountPlans","Prophy Frequency"),discountPlan.ProphyFreqLimit.ToString()));
				}
				if(discountPlan.FluorideFreqLimit>=0) {
					_gridIns.ListGridRows.Add(new GridRow(Lan.g("TableDiscountPlans","Fluoride Frequency"),discountPlan.FluorideFreqLimit.ToString()));
				}
				if(discountPlan.PerioFreqLimit>=0) {
					_gridIns.ListGridRows.Add(new GridRow(Lan.g("TableDiscountPlans","Perio Frequency"),discountPlan.PerioFreqLimit.ToString()));
				}
				if(discountPlan.LimitedExamFreqLimit>=0) {
					_gridIns.ListGridRows.Add(new GridRow(Lan.g("TableDiscountPlans","Limited Frequency"),discountPlan.LimitedExamFreqLimit.ToString()));
				}
				if(discountPlan.XrayFreqLimit>=0) {
					_gridIns.ListGridRows.Add(new GridRow(Lan.g("TableDiscountPlans","X-Ray Frequency"),discountPlan.XrayFreqLimit.ToString()));
				}
				if(discountPlan.PAFreqLimit>=0) {
					_gridIns.ListGridRows.Add(new GridRow(Lan.g("TableDiscountPlans","PA Frequency"),discountPlan.PAFreqLimit.ToString()));
				}
				_gridIns.ListGridRows.Add(rowDiscount);
				rowDiscount=new GridRow();
				rowDiscount.Cells.Add(Lan.g("TableDiscountPlans","Date Term"));
				if(_discountPlanSubCur.DateTerm.Year < 1880) {
					rowDiscount.Cells.Add("");
				}
				else {
					rowDiscount.Cells.Add(_discountPlanSubCur.DateTerm.ToShortDateString());
					//Here is where we would add colors if we wanted an indicator
					if(_discountPlanSubCur.DateTerm<DateTime.Today) {
						rowDiscount.Bold=true;
						rowDiscount.ColorBackG=Color.LightSalmon;
					}
				}
				_gridIns.ListGridRows.Add(rowDiscount);
				//add in grid row for plan note for the disount plan
				rowDiscount=new GridRow();
				rowDiscount.Cells.Add(Lan.g("TableDiscountPlans","Plan Note"));
				GridCell noteCell = new GridCell();
				noteCell.Text=discountPlan.PlanNote;
				noteCell.Bold=YN.Yes;
				noteCell.ColorText=Color.Red;
				rowDiscount.Cells.Add(noteCell);
				_gridIns.ListGridRows.Add(rowDiscount);
				//add grid row for subscriber note for the discount plan's subscriber
				rowDiscount=new GridRow();
				rowDiscount.Cells.Add(Lan.g("TableDiscountPlans","Subscriber Note"));
				noteCell = new GridCell();
				noteCell.Text=_discountPlanSubCur.SubNote;
				noteCell.Bold=YN.Yes;
				noteCell.ColorText=Color.Red;
				rowDiscount.Cells.Add(noteCell);
				_gridIns.ListGridRows.Add(rowDiscount);
				_gridIns.EndUpdate();
				return;
			}
			else {
				_gridIns.Title=Lan.g(this,"Insurance Plans");
			}
			if(_listPatPlans.Count==0){
				_gridIns.BeginUpdate();
				_gridIns.Columns.Clear();
				_gridIns.ListGridRows.Clear();
				_gridIns.EndUpdate();
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
			_gridIns.BeginUpdate();
			_gridIns.Columns.Clear();
			_gridIns.ListGridRows.Clear();
			GridColumn col;
			col=new GridColumn("",150);
			_gridIns.Columns.Add(col);
			int dentalOrdinal=1;
			for(int i=0;i<_listPatPlans.Count;i++) {
				if(listInsPlans[i].IsMedical) {
					col=new GridColumn(Lan.g("TableCoverage","Medical"),170);
					_gridIns.Columns.Add(col);
				}
				else { //dental
					if(dentalOrdinal==1) {
						col=new GridColumn(Lan.g("TableCoverage","Primary"),170);
						_gridIns.Columns.Add(col);
					}
					else if(dentalOrdinal==2) {
						col=new GridColumn(Lan.g("TableCoverage","Secondary"),170);
						_gridIns.Columns.Add(col);
					}
					else {
						col=new GridColumn(Lan.g("TableCoverage","Other"),170);
						_gridIns.Columns.Add(col);
					}
					dentalOrdinal++;
				}
			}
			OpenDental.UI.GridRow row=new GridRow();
			//subscriber
			row.Cells.Add(Lan.g("TableCoverage","Subscriber"));
			for(int i=0;i<_listPatPlans.Count;i++) {
				row.Cells.Add(_famCur.GetNameInFamFL(listInsSubs[i].Subscriber));
			}
			row.ColorBackG=listDefs[(int)DefCatMiscColors.FamilyModuleCoverage].ItemColor;
			_gridIns.ListGridRows.Add(row);
			//subscriber ID
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Subscriber ID"));
			for(int i=0;i<_listPatPlans.Count;i++) {
				row.Cells.Add(listInsSubs[i].SubscriberID);
			}
			row.ColorBackG=listDefs[(int)DefCatMiscColors.FamilyModuleCoverage].ItemColor;
			_gridIns.ListGridRows.Add(row);
			//relationship
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Rel'ship to Sub"));
			for(int i=0;i<_listPatPlans.Count;i++){
				row.Cells.Add(Lan.g("enumRelat",_listPatPlans[i].Relationship.ToString()));
			}
			row.ColorBackG=listDefs[(int)DefCatMiscColors.FamilyModuleCoverage].ItemColor;
			_gridIns.ListGridRows.Add(row);
			//patient ID
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Patient ID"));
			for(int i=0;i<_listPatPlans.Count;i++) {
				row.Cells.Add(_listPatPlans[i].PatID);
			}
			row.ColorBackG=listDefs[(int)DefCatMiscColors.FamilyModuleCoverage].ItemColor;
			_gridIns.ListGridRows.Add(row);
			//pending
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Pending"));
			for(int i=0;i<_listPatPlans.Count;i++) {
				if(_listPatPlans[i].IsPending){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
			}
			row.ColorBackG=listDefs[(int)DefCatMiscColors.FamilyModuleCoverage].ItemColor;
			row.ColorLborder=Color.Black;
			_gridIns.ListGridRows.Add(row);
			//employer
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Employer"));
			for(int i=0;i<_listPatPlans.Count;i++) {
				row.Cells.Add(Employers.GetName(listInsPlans[i].EmployerNum));
			}
			_gridIns.ListGridRows.Add(row);
			//carrier
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Carrier"));
			for(int i=0;i<_listPatPlans.Count;i++) {
				row.Cells.Add(InsPlans.GetCarrierName(listInsPlans[i].PlanNum,listInsPlans));
			}
			_gridIns.ListGridRows.Add(row);
			//group name
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Group Name"));
			for(int i=0;i<_listPatPlans.Count;i++) {
				row.Cells.Add(listInsPlans[i].GroupName);
			}
			_gridIns.ListGridRows.Add(row);
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
			_gridIns.ListGridRows.Add(row);
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
						}
						else {
							row.Cells.Add(Lan.g(this,"PPO Percentage"));
						}
						break;
					case "f":
						row.Cells.Add(Lan.g(this,"Medicaid or Flat Co-pay"));
						break;
					case "c":
						row.Cells.Add(Lan.g(this,"Capitation"));
						break;
				}
			}
			_gridIns.ListGridRows.Add(row);
			//fee schedule
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Fee Schedule"));
			for(int i=0;i<listInsPlans.Count;i++) {
				row.Cells.Add(FeeScheds.GetDescription(listInsPlans[i].FeeSched));
			}
			row.ColorLborder=Color.Black;
			_gridIns.ListGridRows.Add(row);
			//Calendar vs service year------------------------------------------------------------------------------------
			row=new GridRow();
			row.Cells.Add(Lan.g("TableCoverage","Benefit Period"));
			for(int i=0;i<listInsPlans.Count;i++) {
				if(listInsPlans[i].MonthRenew==0) {
					row.Cells.Add(Lan.g("TableCoverage","Calendar Year"));
				}
				else {
					DateTime dateService=new DateTime(2000,listInsPlans[i].MonthRenew,1);
					row.Cells.Add(Lan.g("TableCoverage","Service year begins:")+" "+dateService.ToString("MMMM"));
				}
			}
			_gridIns.ListGridRows.Add(row);
			//Benefits-----------------------------------------------------------------------------------------------------
			List <Benefit> listBensForPat=_loadData.ListBenefits;
			Benefit[,] benefitMatrix=Benefits.GetDisplayMatrix(listBensForPat,_listPatPlans,_listInsSubs);
			string strDesc;
			string strVal;
			ProcedureCode procCode=null;
			for(int y=0;y<benefitMatrix.GetLength(1);y++) {//rows
				bool hasSpecialFreqAdded=false;
				bool hasSpecialAgeLimitAdded=false;
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
					procCode=ProcedureCodes.GetProcCode(benefitMatrix[x,y].CodeNum);
					if(benefitMatrix[x,y].BenefitType==InsBenefitType.CoInsurance && benefitMatrix[x,y].Percent!=-1) {
						if(benefitMatrix[x,y].CodeNum==0) {
							strDesc+=CovCats.GetDesc(benefitMatrix[x,y].CovCatNum)+" % ";
						}
						else {
							strDesc+=procCode.ProcCode+"-"+procCode.AbbrDesc+" % ";
						}
					}
					else if(benefitMatrix[x,y].BenefitType==InsBenefitType.Deductible) {
						strDesc+=Lan.g(this,"Deductible")+" "+CovCats.GetDesc(benefitMatrix[x,y].CovCatNum)+" ";
					}
					else if(benefitMatrix[x,y].BenefitType==InsBenefitType.Limitations
						&& benefitMatrix[x,y].QuantityQualifier==BenefitQuantity.None
						&& (benefitMatrix[x,y].TimePeriod==BenefitTimePeriod.ServiceYear
						|| benefitMatrix[x,y].TimePeriod==BenefitTimePeriod.CalendarYear))
					{//annual max
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
					else if(Benefits.IsExamFrequency(benefitMatrix[x,y]) && !_gridIns.ListGridRows.Any(x => x.Cells[0].Text.Contains(Lan.g(this,"Exam frequency")+" "))) {
						strDesc+=Lan.g(this,"Exam frequency")+" ";
						hasSpecialFreqAdded=true;
					}
					else if(Benefits.IsBitewingFrequency(benefitMatrix[x,y]) 
						&& !_gridIns.ListGridRows.Any(x => x.Cells[0].Text.Contains(Lan.g(this,"BW frequency")+" "))
						&& (benefitMatrix[x,y].CodeNum==ProcedureCodes.GetCodeNum(ProcedureCodes.BitewingCode) 
							|| listBensForPat.FirstOrDefault(x => ProcedureCodes.GetCodeNum(ProcedureCodes.BitewingCode)==x.CodeNum)==null)) 
					{
						strDesc+=Lan.g(this,"BW frequency")+" ";
						hasSpecialFreqAdded=true;
					}
					else if(Benefits.IsPanoFrequency(benefitMatrix[x,y]) 
						&& !_gridIns.ListGridRows.Any(x => x.Cells[0].Text.Contains(Lan.g(this,"Pano/FMX frequency")+" "))
						&& (benefitMatrix[x,y].CodeNum==ProcedureCodes.GetCodeNum(ProcedureCodes.PanoCode) 
							|| listBensForPat.FirstOrDefault(x => ProcedureCodes.GetCodeNum(ProcedureCodes.PanoCode)==x.CodeNum)==null)) 
					{
						strDesc+=Lan.g(this,"Pano/FMX frequency")+" ";
						hasSpecialFreqAdded=true;
					}
					else if(Benefits.IsCancerScreeningFrequency(benefitMatrix[x,y]) 
						&& !_gridIns.ListGridRows.Any(x => x.Cells[0].Text.Contains(Lan.g(this,"Cancer Screening frequency")+" "))
						&& (benefitMatrix[x,y].CodeNum==ProcedureCodes.GetCodeNum(ProcedureCodes.CancerScreeningCode) 
							|| listBensForPat.FirstOrDefault(x => ProcedureCodes.GetCodeNum(ProcedureCodes.CancerScreeningCode)==x.CodeNum)==null)) 
					{
						strDesc+=Lan.g(this,"Cancer Screening frequency")+" ";
						hasSpecialFreqAdded=true;
					}
					else if(Benefits.IsProphyFrequency(benefitMatrix[x,y]) 
						&& !_gridIns.ListGridRows.Any(x => x.Cells[0].Text.Contains(Lan.g(this,"Prophy frequency")+" "))
						&& (benefitMatrix[x,y].CodeNum==ProcedureCodes.GetCodeNum(ProcedureCodes.ProphyCode) 
							|| listBensForPat.FirstOrDefault(x => ProcedureCodes.GetCodeNum(ProcedureCodes.ProphyCode)==x.CodeNum)==null)) 
					{
						strDesc+=Lan.g(this,"Prophy frequency")+" ";
						hasSpecialFreqAdded=true;
					}
					else if(Benefits.IsFlourideFrequency(benefitMatrix[x,y]) 
						&& !_gridIns.ListGridRows.Any(x => x.Cells[0].Text.Contains(Lan.g(this,"Fluoride frequency")+" "))
						&& (benefitMatrix[x,y].CodeNum==ProcedureCodes.GetCodeNum(ProcedureCodes.FlourideCode) 
							|| listBensForPat.FirstOrDefault(x => ProcedureCodes.GetCodeNum(ProcedureCodes.FlourideCode)==x.CodeNum)==null)) 
					{
						strDesc+=Lan.g(this,"Fluoride frequency")+" ";
						hasSpecialFreqAdded=true;
					}
					else if(Benefits.IsFlourideAgeLimit(benefitMatrix[x,y])) {
						strDesc+=Lan.g(this,"Fluoride age limit")+" ";
						hasSpecialAgeLimitAdded=true;
					}
					else if(Benefits.IsSealantFrequency(benefitMatrix[x,y]) 
						&& !_gridIns.ListGridRows.Any(x => x.Cells[0].Text.Contains(Lan.g(this,"Sealant frequency")+" "))
						&& (benefitMatrix[x,y].CodeNum==ProcedureCodes.GetCodeNum(ProcedureCodes.SealantCode) 
							|| listBensForPat.FirstOrDefault(x => ProcedureCodes.GetCodeNum(ProcedureCodes.SealantCode)==x.CodeNum)==null)) 
					{
						strDesc+=Lan.g(this,"Sealant frequency")+" ";
						hasSpecialFreqAdded=true;
					}
					else if(Benefits.IsSealantAgeLimit(benefitMatrix[x,y])) {
						strDesc+=Lan.g(this,"Sealant age limit")+" ";
						hasSpecialAgeLimitAdded=true;
					}
					else if(Benefits.IsCrownFrequency(benefitMatrix[x,y]) 
						&& !_gridIns.ListGridRows.Any(x => x.Cells[0].Text.Contains(Lan.g(this,"Crown frequency")+" "))
						&& (benefitMatrix[x,y].CodeNum==ProcedureCodes.GetCodeNum(ProcedureCodes.CrownCode) 
							|| listBensForPat.FirstOrDefault(x => ProcedureCodes.GetCodeNum(ProcedureCodes.CrownCode)==x.CodeNum)==null)) 
					{
						strDesc+=Lan.g(this,"Crown frequency")+" ";
						hasSpecialFreqAdded=true;
					}
					else if(Benefits.IsSRPFrequency(benefitMatrix[x,y]) 
						&& !_gridIns.ListGridRows.Any(x => x.Cells[0].Text.Contains(Lan.g(this,"SRP frequency")+" "))
						&& (benefitMatrix[x,y].CodeNum==ProcedureCodes.GetCodeNum(ProcedureCodes.SRPCode) 
							|| listBensForPat.FirstOrDefault(x => ProcedureCodes.GetCodeNum(ProcedureCodes.SRPCode)==x.CodeNum)==null)) 
					{
						strDesc+=Lan.g(this,"SRP frequency")+" ";
						hasSpecialFreqAdded=true;
					}
					else if(Benefits.IsFullDebridementFrequency(benefitMatrix[x,y]) 
						&& !_gridIns.ListGridRows.Any(x => x.Cells[0].Text.Contains(Lan.g(this,"Full Debridement frequency")+" "))
						&& (benefitMatrix[x,y].CodeNum==ProcedureCodes.GetCodeNum(ProcedureCodes.FullDebridementCode) 
							|| listBensForPat.FirstOrDefault(x => ProcedureCodes.GetCodeNum(ProcedureCodes.FullDebridementCode)==x.CodeNum)==null)) 
					{
						strDesc+=Lan.g(this,"Full Debridement frequency")+" ";
						hasSpecialFreqAdded=true;
					}
					else if(Benefits.IsPerioMaintFrequency(benefitMatrix[x,y]) 
						&& !_gridIns.ListGridRows.Any(x => x.Cells[0].Text.Contains(Lan.g(this,"Perio Maint frequency")+" "))
						&& (benefitMatrix[x,y].CodeNum==ProcedureCodes.GetCodeNum(ProcedureCodes.PerioMaintCode) 
							|| listBensForPat.FirstOrDefault(x => ProcedureCodes.GetCodeNum(ProcedureCodes.PerioMaintCode)==x.CodeNum)==null)) 
					{
						strDesc+=Lan.g(this,"Perio Maint frequency")+" ";
						hasSpecialFreqAdded=true;
					}
					else if(Benefits.IsDenturesFrequency(benefitMatrix[x,y]) 
						&& !_gridIns.ListGridRows.Any(x => x.Cells[0].Text.Contains(Lan.g(this,"Dentures frequency")+" "))
						&& (benefitMatrix[x,y].CodeNum==ProcedureCodes.GetCodeNum(ProcedureCodes.DenturesCode) 
							|| listBensForPat.FirstOrDefault(x => ProcedureCodes.GetCodeNum(ProcedureCodes.DenturesCode)==x.CodeNum)==null)) 
					{
						strDesc+=Lan.g(this,"Dentures frequency")+" ";
						hasSpecialFreqAdded=true;
					}
					else if(Benefits.IsImplantFrequency(benefitMatrix[x,y]) 
						&& !_gridIns.ListGridRows.Any(x => x.Cells[0].Text.Contains(Lan.g(this,"Implants frequency")+" "))
						&& (benefitMatrix[x,y].CodeNum==ProcedureCodes.GetCodeNum(ProcedureCodes.ImplantCode) 
							|| listBensForPat.FirstOrDefault(x => ProcedureCodes.GetCodeNum(ProcedureCodes.ImplantCode)==x.CodeNum)==null)) 
					{
						strDesc+=Lan.g(this,"Implants frequency")+" ";
						hasSpecialFreqAdded=true;
					}
					else if(benefitMatrix[x,y].CodeNum==0 && procCode.AbbrDesc!=null) {//e.g. flo
						strDesc+=procCode.AbbrDesc+" ";
					}
					else{
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
					if(benefitMatrix[x,y].BenefitType==InsBenefitType.Exclusions
						|| benefitMatrix[x,y].BenefitType==InsBenefitType.Limitations
						&& !(hasSpecialFreqAdded || hasSpecialAgeLimitAdded)) 
					{
						if(benefitMatrix[x,y].CodeNum != 0) {
							procCode=ProcedureCodes.GetProcCode(benefitMatrix[x,y].CodeNum);
							strVal+=procCode.ProcCode+"-"+procCode.AbbrDesc+" ";
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
					else{
						if(benefitMatrix[x,y].QuantityQualifier!=BenefitQuantity.None && !hasSpecialAgeLimitAdded) {//e.g. flo
							strVal+=Lan.g("enumBenefitQuantity",benefitMatrix[x,y].QuantityQualifier.ToString())+" ";
						}
						if(benefitMatrix[x,y].Quantity!=0) {
							strVal+=benefitMatrix[x,y].Quantity.ToString()+" ";
						}
						if(hasSpecialAgeLimitAdded) {
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
				_gridIns.ListGridRows.Add(row);
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
			_gridIns.ListGridRows.Add(row);
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
			_gridIns.ListGridRows.Add(row);
			//InsHist
			Dictionary<long,InsProcHist> dictInsProcHist=_listPatPlans.Select(x => x.InsSubNum).Distinct()
				.ToDictionary(x => x,x => new InsProcHist(Procedures.GetDictInsHistProcs(_patCur.PatNum,x,out List<ClaimProc> listClaimProcs),listClaimProcs));
			foreach(PrefName prefName in Prefs.GetInsHistPrefNames()) {
				row=new GridRow();
				row.Cells.Add(Lan.g("TableCoverage",prefName.GetDescription()));
				foreach(PatPlan patPlan in _listPatPlans) {
					DateTime procDate=DateTime.MinValue;
					if(dictInsProcHist.TryGetValue(patPlan.InsSubNum,out InsProcHist insProcHist)
						&& insProcHist.DictInsHistProcs.TryGetValue(prefName,out Procedure proc)
						&& proc!=null
						&& insProcHist.ListClaimProcs
							.Exists(x => x.InsSubNum==patPlan.InsSubNum && x.Status.In(ClaimProcStatus.InsHist,ClaimProcStatus.Received) && x.ProcNum==proc.ProcNum))
					{
						procDate=proc.ProcDate;
					}
					row.Cells.Add(new GridCell(procDate.Year>1880?procDate.ToShortDateString():Lan.g("TableCoverage","No History")));
				}
				row.Tag=prefName.ToString();//Tag with prefname
				_gridIns.ListGridRows.Add(row);
			}
			_gridIns.EndUpdate();
		}

		private void ToolButDiscount_Click() {
			if(_listPatPlans.Count>0) {
				MsgBox.Show(this,"Cannot add discount plan when patient has insurance.");
				return;
			}
			if(_discountPlanSubCur==null) {//Patient does not have a discount plan.
				//Let the user pick which discount plan the patient should subscribe to.
				using FormDiscountPlans formDiscountPlans=new FormDiscountPlans();
				formDiscountPlans.IsSelectionMode=true;
				if(formDiscountPlans.ShowDialog()!=DialogResult.OK) {
					return;
				}
				//Give the user an opportunity to edit the subscription.
				using FormDiscountPlanSubEdit formDiscountPlanSubEdit=new FormDiscountPlanSubEdit();
				formDiscountPlanSubEdit.DiscountPlanCur=formDiscountPlans.DiscountPlanSelected;
				formDiscountPlanSubEdit.PatNum=_patCur.PatNum;
				if(formDiscountPlanSubEdit.ShowDialog()!=DialogResult.OK || formDiscountPlanSubEdit.DiscountPlanSubCur==null) {
					return;//User either clicked Cancel or Drop, nothing to do.
				}
				_discountPlanSubCur=formDiscountPlanSubEdit.DiscountPlanSubCur;
			}
			else {
				using FormDiscountPlanSubEdit formDiscountPlanSubEdit=new FormDiscountPlanSubEdit();
				formDiscountPlanSubEdit.DiscountPlanSubCur=_discountPlanSubCur;
				if(formDiscountPlanSubEdit.ShowDialog()!=DialogResult.OK) {
					return;
				}
				_discountPlanSubCur=formDiscountPlanSubEdit.DiscountPlanSubCur;
			}
			//Update all active and inactive treatment plans regardless if the user added or dropped a discount plan.
			TreatPlans.UpdateTreatmentPlanType(_patCur);
			FillInsData();
		}

		private void ToolButIns_Click() {
			if(_discountPlanSubCur!=null) {
				MsgBox.Show(this,"Cannot add insurance if patient has a discount plan.");
				return;
			}
			DialogResult result=MessageBox.Show(Lan.g(this,"Is this patient the subscriber?"),"",MessageBoxButtons.YesNoCancel);
			if(result==DialogResult.Cancel) {
				return;
			}
			//Pick a subscriber------------------------------------------------------------------------------------------------
			Patient patSubscriber;
			if(result==DialogResult.Yes) {//current patient is subscriber
				patSubscriber=_patCur.Copy();
			}
			else {//patient is not subscriber
				//show list of patients in this family
				using FormSubscriberSelect formSubsciberSelect=new FormSubscriberSelect(_famCur);
				formSubsciberSelect.ShowDialog();
				if(formSubsciberSelect.DialogResult==DialogResult.Cancel) {
					return;
				}
				patSubscriber=Patients.GetPat(formSubsciberSelect.SelectedPatNum);
			}
			//Subscriber has been chosen. Now, pick a plan-------------------------------------------------------------------
			InsPlan insPlan=null;
			InsSub insSub=null;
			bool isNewPlan=false;
			List<InsSub> listInsSubs=InsSubs.GetListForSubscriber(patSubscriber.PatNum);
			if(listInsSubs.Count==0) {
				isNewPlan=true;
			}
			else {
				using FormInsSelectSubscr formInsSelectSubscr=new FormInsSelectSubscr(patSubscriber.PatNum,_patCur.PatNum);
				formInsSelectSubscr.ShowDialog();
				if(formInsSelectSubscr.DialogResult==DialogResult.Cancel) {
					return;
				}
				if(formInsSelectSubscr.SelectedInsSubNum==0) {//'New' option selected.
					isNewPlan=true;
				}
				else {
					insSub=InsSubs.GetSub(formInsSelectSubscr.SelectedInsSubNum,listInsSubs);
					insPlan=InsPlans.GetPlan(insSub.PlanNum,new List<InsPlan>());
				}
			}
			//New plan was selected instead of an existing plan.  Create the plan--------------------------------------------
			if(isNewPlan) {
				insPlan=new InsPlan();
				insPlan.EmployerNum=patSubscriber.EmployerNum;
				insPlan.PlanType="";
				InsPlans.Insert(insPlan);
				insSub=new InsSub();
				insSub.PlanNum=insPlan.PlanNum;
				insSub.Subscriber=patSubscriber.PatNum;
				insSub.SubscriberID=patSubscriber.MedicaidID;
				insSub.ReleaseInfo=true;
				insSub.AssignBen=PrefC.GetBool(PrefName.InsDefaultAssignBen);
				InsSubs.Insert(insSub);
				Benefit benefit;
				foreach(CovCat covCat in CovCats.GetWhere(x => x.DefaultPercent!=-1,true)) {
					benefit=new Benefit();
					benefit.BenefitType=InsBenefitType.CoInsurance;
					benefit.CovCatNum=covCat.CovCatNum;
					benefit.PlanNum=insPlan.PlanNum;
					benefit.Percent=covCat.DefaultPercent;
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
			patplan.PatNum=_patCur.PatNum;
			patplan.InsSubNum=insSub.InsSubNum;
			patplan.Relationship=Relat.Self;
			PatPlans.Insert(patplan);
			//Then, display insPlanEdit to user-------------------------------------------------------------------------------
			using FormInsPlan formInsPlan=new FormInsPlan(insPlan,patplan,insSub);
			formInsPlan.IsNewPlan=isNewPlan;
			formInsPlan.IsNewPatPlan=true;
			if(formInsPlan.ShowDialog()!=DialogResult.Cancel) {
				SecurityLogs.MakeLogEntry(Permissions.PatPlanCreate,_patCur.PatNum,"Inserted new PatPlan for patient. InsPlanNum: "+formInsPlan.GetPlanCurNum());
				//Update users treatment plans to tie in to insurance
				TreatPlans.UpdateTreatmentPlanType(_patCur);
			}//this updates estimates also.
			//if cancel, then patplan is deleted from within that dialog.
			//if cancel, and planIsNew, then plan and benefits are also deleted.
			ModuleSelected(_patCur.PatNum);
		}
		#endregion Methods - Private - GridIns

		#region Methods - Private - Patient Clones
		private void FillGridPatientClones() {
			_gridPatientClones.BeginUpdate();
			_gridPatientClones.Columns.Clear();
			_gridPatientClones.Columns.Add(new GridColumn(Lan.g(_gridPatientClones.TranslationName,"Name"),150));
			if(PrefC.HasClinicsEnabled) {
				_gridPatientClones.Columns.Add(new GridColumn(Lan.g(_gridPatientClones.TranslationName,"Clinic"),80));
			}
			_gridPatientClones.Columns.Add(new GridColumn(Lan.g(_gridPatientClones.TranslationName,"Specialty"),150){ IsWidthDynamic=true });
			_gridPatientClones.ListGridRows.Clear();
			if(_patCur==null) {
				_gridPatientClones.EndUpdate();
				return;
			}
			int selectedIndex=-1;
			GridRow row;
			foreach(KeyValuePair<Patient,Def> cloneAndSpecialty in _dictCloneSpecialty) {
				//Never add deleted patients to the grid.  Deleted patients should not be selectable.
				if(cloneAndSpecialty.Key.PatStatus==PatientStatus.Deleted) {
					continue;
				}
				row=new GridRow();
				row.Cells.Add(cloneAndSpecialty.Key.GetNameLF());
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(cloneAndSpecialty.Key.ClinicNum));
				}
				//Check for null because an office could have just turned on clinics and a specialty would not have been required prior.
				row.Cells.Add((cloneAndSpecialty.Value==null)?"":cloneAndSpecialty.Value.ItemName);
				row.Tag=cloneAndSpecialty.Key;
				//If we are about to add the clone that is currently selected, save the index of said patient so that we can select them after the update.
				if(_patCur!=null && cloneAndSpecialty.Key.PatNum==_patCur.PatNum) {
					selectedIndex=_gridPatientClones.ListGridRows.Count;
				}
				_gridPatientClones.ListGridRows.Add(row);
			}
			//The first entry will always be the original or master patient which we want to stand out a little bit much like the Super Family grid.
			if(_gridPatientClones.ListGridRows.Count>0) {
				_gridPatientClones.ListGridRows[0].Cells[0].Bold=YN.Yes;
				_gridPatientClones.ListGridRows[0].Cells[0].ColorText=Color.OrangeRed;
			}
			_gridPatientClones.EndUpdate();
			//The grid has finished refreshing and can now have it's selected index changed.
			if(selectedIndex>-1) {
				_gridPatientClones.SetSelected(selectedIndex,true);
			}
		}

		///<summary>Returns a boolean based on if the current state of the Family module is ready for acting on behalf of the clone feature.
		///If something is not ready for clone action to be taken a message will show to the user and false will be returned.</summary>
		private bool IsValidForCloneAction() {
			if(_patCur==null) {
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
			if(PatientLinks.IsPatientAClone(_patCur.PatNum)) {
				long patNumMaster=PatientLinks.GetOriginalPatNumFromClone(_patCur.PatNum);
				Patient patientMaster=Patients.GetPat(patNumMaster);
				//Double check that the original or master patient was found.
				if(patientMaster==null) {
					MsgBox.Show(this,"The original patient cannot be found in order to create additional clones.  Please call support.");
					return;
				}
				formCloneAdd=new FormCloneAdd(patientMaster);
			}
			else {//The currently selected patient is the original or master patient.
				formCloneAdd=new FormCloneAdd(_patCur,_famCur,_listInsPlans,_listInsSubs,_listBenefits);
			}
			formCloneAdd.ShowDialog();
			//At this point we know that we have all information regarding the original or master patient.
			if(formCloneAdd.DialogResult!=DialogResult.OK) {
				return;
			}
			//Refresh the module with the new clone if one was created.
			long patNum=_patCur.PatNum;
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
			if(PatientLinks.IsPatientAClone(_patCur.PatNum)) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Break the currently selected clone from the current clone group?")) {
					return;
				}
				PatientLinks.DeletePatNumTos(_patCur.PatNum,PatientLinkType.Clone);
			}
			else {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"The original patient clone is currently selected.  "
					+"Breaking the original patient clone will cause all clone links in the current clone group to be broken.\r\n"
					+"Continue anyway?")) 
				{
					return;
				}
				PatientLinks.DeletePatNumFroms(_patCur.PatNum,PatientLinkType.Clone);
			}
			ModuleSelected(_patCur.PatNum);
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
			string strDataUpdated=Patients.SynchClonesWithPatient(_patCur,_famCur,_listInsPlans,_listInsSubs,_listBenefits,_listPatPlans);
			ModuleSelected(_patCur.PatNum);
			if(string.IsNullOrWhiteSpace(strDataUpdated)) {
				strDataUpdated=Lan.g(this,"No changes were made, data already in synch.");
			}
			new MsgBoxCopyPaste(strDataUpdated).Show();
		}
		#endregion Methods - Private - Patient Clones

		#region Methods - Private - Other
		private void FillPatientPicture() {
			_odPictureBoxPat.Image?.Dispose();
			_odPictureBoxPat.Image=null;
			_odPictureBoxPat.TextNullImage=Lan.g(this,"Patient Picture Unavailable");
			if(_patCur==null || 
				PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {//Do not use patient image when A to Z folders are disabled.
				return;
			}
			try {
				_odPictureBoxPat.Image?.Dispose();
				if(_loadData.HasPatPict==YN.Unknown) {
					_odPictureBoxPat.Image=Documents.GetPatPict(_patCur.PatNum,ImageStore.GetPatientFolder(_patCur,ImageStore.GetPreferredAtoZpath()));
				}
				else {
					_odPictureBoxPat.Image=Documents.GetPatPict(_patCur.PatNum,ImageStore.GetPatientFolder(_patCur,ImageStore.GetPreferredAtoZpath()),_loadData.PatPict);
				}
			}
			catch {
			}
		}

		///<summary>Shows warning if the patient's guarantor has been sent to TSI. Returns true if the patient has been sent to TSI and the user wants to cancel the move. Otherwise, returns false.</summary>
		private bool IsGuarantorTSI() {
			if(!TsiTransLogs.HasGuarBeenSentToTSI(_famCur.Guarantor)) {
				return false;
			}
			return !MsgBox.Show(this,MsgBoxButtons.OKCancel,"The guarantor of this family has been sent to TSI for a past due balance. "
				+"Moving a family member could change the balance and result in a charge by TSI. "
				+"We recommend canceling TSI professional collection before moving a family member.\r\nContinue with the move?");
		}

		private bool MovePats(Patient patOld,Family famCur=null) {
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
			using FormPatientSelect formPatSelect=new FormPatientSelect();
			formPatSelect.SelectionModeOnly=true;
			formPatSelect.ShowDialog();
			if(formPatSelect.DialogResult!=DialogResult.OK) {
				return false;
			}
			Patient patInNewFam=Patients.GetPat(formPatSelect.SelectedPatNum);
			if(famCur!=null) {//Move all family members marked as merged silently (The family should only contain guarantor and any merged pats at this point)
				foreach(Patient pat in famCur.ListPats) {
					if(pat.PatNum==patOld.PatNum) {
						continue;//Don't move current pat yet
					}
					Patient patOldFam=pat.Copy();
					pat.Guarantor=patInNewFam.Guarantor;
					pat.SuperFamily=patInNewFam.SuperFamily;
					Patients.Update(pat,patOldFam);
				}
			}
			if(_patCur.SuperFamily!=patInNewFam.SuperFamily) {//If they are moving into or out of a superfamily
				if(_patCur.SuperFamily!=0) {//If they are currently in a SuperFamily and moving out.  Otherwise, no superfamily popups to worry about.
					Popups.CopyForMovingSuperFamily(_patCur,patInNewFam.SuperFamily);
				}
			}
			_patCur.Guarantor=patInNewFam.Guarantor;
			_patCur.SuperFamily=patInNewFam.SuperFamily;
			Patients.Update(_patCur,patOld);
			_famCur=Patients.GetFamily(_patCur.PatNum);
			Patients.CombineGuarantors(_famCur,_patCur);
			return true;
		}

		private void RefreshModuleData(long patNum) {
			if(patNum==0)	{
				_patCur=null;
				_patNoteCur=null;
				_discountPlanSubCur=null;
				_famCur=null;
				_listPatPlans=new List<PatPlan>(); 
				return;
			}
			if(_patCur!=null && _discountPlanSubCur!=null && _loadData.ListPatPlans.Count>0) {
				DiscountPlanSubs.DeleteForPatient(_patCur.PatNum);
				string logText=Lan.g(this,"The discount plan")+" "+DiscountPlans.GetPlan(_discountPlanSubCur.DiscountPlanNum).Description+" "+Lan.g(this,"was automatically dropped due to patient having an insuarance plan.");
				SecurityLogs.MakeLogEntry(Permissions.DiscountPlanAddDrop,_patCur.PatNum,logText);
				string messageText="Discount plan removed due to patient having both an insurance plan and a discount plan. "
					+"If the patient should have a discount plan, please first remove all insurance plans before adding a discount plan.";
				MsgBox.Show(messageText);
			}
			bool doCreateSecLog=false;
			if(_patNumLast!=patNum) {
				doCreateSecLog=true;
				_patNumLast=patNum;//Stops module from making too many logs
			}
			_loadData=FamilyModules.GetLoadData(patNum,doCreateSecLog);
			_famCur=_loadData.Fam;
			_patCur=_loadData.Pat;
			_discountPlanSubCur=_loadData.DiscountPlanSub;
			_patNoteCur=_loadData.PatNote;
			_listInsSubs=_loadData.ListInsSubs;
			_listInsPlans=_loadData.ListInsPlans;
			_listPatPlans=_loadData.ListPatPlans;
			_listBenefits=_loadData.ListBenefits;
			_listRecalls=_loadData.ListRecalls;
			_arrayPatFields=_loadData.ArrPatFields;
			_listPatSuperFamilyMembers=_loadData.SuperFamilyMembers;
			_listPatSuperFamilyGuarantors=_loadData.SuperFamilyGuarantors;
			_dictCloneSpecialty=_loadData.DictCloneSpecialities;
			//Takes the preference string and converts it to an enum object
			_sortStratSuperFam=(SortStrategy)PrefC.GetInt(PrefName.SuperFamSortStrategy);
		}

		private void RefreshModuleScreen() {
			if(_patCur!=null){//if there is a patient
				//ToolBarMain.Buttons["Recall"].Enabled=true;
				_toolBarMain.Buttons["Add"].Enabled=true;
				_toolBarMain.Buttons["Delete"].Enabled=true;
				_toolBarMain.Buttons["Guarantor"].Enabled=true;
				_toolBarMain.Buttons["Move"].Enabled=true;
				if(_toolBarMain.Buttons["Ins"]!=null && !PrefC.GetBool(PrefName.EasyHideInsurance)) {
					_toolBarMain.Buttons["Ins"].Enabled=true;
					_toolBarMain.Buttons["Discount"].Enabled=true;
				}
				//Only show Superfamily and Patient Clone containers if either feature is on and there is information for the enabled feature to show.
				//The program will still need to be restarted to ensure all UI changes are accurately reflected.
				bool doShowPanelForSuperfamilies=PrefC.GetBool(PrefName.ShowFeatureSuperfamilies) && _patCur.SuperFamily!=0;
				bool doShowPanelForPatientClone=PrefC.GetBool(PrefName.ShowFeaturePatientClone) && _dictCloneSpecialty!=null && _dictCloneSpecialty.Count > 1;
				if(doShowPanelForSuperfamilies || doShowPanelForPatientClone) {
					_splitContainerSuperClones.Visible=true;
					LayoutManager.MoveLocation(_gridIns,new Point(_splitContainerSuperClones.Right+2,_gridIns.Top));
					LayoutManager.MoveWidth(_gridIns,Width-_gridIns.Left);
				}
				else {
					_splitContainerSuperClones.Visible=false;
					LayoutManager.MoveLocation(_gridIns,_splitContainerSuperClones.Location);
					LayoutManager.MoveWidth(_gridIns,Width-_gridIns.Left);
				}
				if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies) && _toolBarMain.Buttons["AddSuper"]!=null) {
					_toolBarMain.Buttons["AddSuper"].Enabled=true;
				}
				if(_patCur.SuperFamily==0 || _toolBarMain.Buttons["AddSuper"]==null) {
					_splitContainerSuperClones.Panel1Collapsed=true;
					_splitContainerSuperClones.Panel1.Hide();
				}
				else {
					_splitContainerSuperClones.Panel1Collapsed=false;
					_splitContainerSuperClones.Panel1.Show();
					_toolBarMain.Buttons["AddSuper"].Enabled=true;
					_toolBarMain.Buttons["RemoveSuper"].Enabled=true;
					_toolBarMain.Buttons["DisbandSuper"].Enabled=true;
				}
				if(PrefC.GetBool(PrefName.ShowFeaturePatientClone)
					&& _toolBarMain.Buttons["AddClone"]!=null)
				{
					_toolBarMain.Buttons["AddClone"].Enabled=true;
				}
				if(_dictCloneSpecialty!=null && _dictCloneSpecialty.Count>1
					&& Patients.IsPatientACloneOrOriginal(_patCur.PatNum)
					&& _toolBarMain.Buttons["SynchClone"]!=null
					&& _toolBarMain.Buttons["BreakClone"]!=null)
				{
					_toolBarMain.Buttons["SynchClone"].Enabled=true;
					_toolBarMain.Buttons["BreakClone"].Enabled=true;
					_splitContainerSuperClones.Panel2Collapsed=false;
					_splitContainerSuperClones.Panel2.Show();
				}
				else {
					_splitContainerSuperClones.Panel2Collapsed=true;
					_splitContainerSuperClones.Panel2.Hide();
					if(_toolBarMain.Buttons["SynchClone"]!=null && _toolBarMain.Buttons["BreakClone"]!=null) {
						_toolBarMain.Buttons["SynchClone"].Enabled=false;
						_toolBarMain.Buttons["BreakClone"].Enabled=false;
					}
				}
				_toolBarMain.Invalidate();
			}
			else {//no patient selected
				//Hide super family and patient clone grids, safe to run even if panel is already hidden.
				_splitContainerSuperClones.Visible=false;
				LayoutManager.MoveLocation(_gridIns,_splitContainerSuperClones.Location);
				LayoutManager.MoveWidth(_gridIns,Width-_gridIns.Left);
				_toolBarMain.Buttons["Add"].Enabled=false;
				_toolBarMain.Buttons["Delete"].Enabled=false;
				_toolBarMain.Buttons["Guarantor"].Enabled=false;
				_toolBarMain.Buttons["Move"].Enabled=false;
				if(_toolBarMain.Buttons["AddSuper"]!=null) {//because the toolbar only refreshes on restart.
					_toolBarMain.Buttons["AddSuper"].Enabled=false;
					_toolBarMain.Buttons["RemoveSuper"].Enabled=false;
					_toolBarMain.Buttons["DisbandSuper"].Enabled=false;
				}
				if(_toolBarMain.Buttons["Ins"]!=null && !PrefC.GetBool(PrefName.EasyHideInsurance)) {
					_toolBarMain.Buttons["Ins"].Enabled=false;
					_toolBarMain.Buttons["Discount"].Enabled=false;
				}
				if(PrefC.GetBool(PrefName.ShowFeaturePatientClone)
					&& _toolBarMain.Buttons["AddClone"]!=null
					&& _toolBarMain.Buttons["SynchClone"]!=null
					&& _toolBarMain.Buttons["BreakClone"]!=null)
				{
					_toolBarMain.Buttons["AddClone"].Enabled=false;
					_toolBarMain.Buttons["SynchClone"].Enabled=false;
					_toolBarMain.Buttons["BreakClone"].Enabled=false;
				}
				_toolBarMain.Invalidate();
			}
			if(PrefC.GetBool(PrefName.EasyHideInsurance)) {
				_gridIns.Visible=false;
			}
			else {
				_gridIns.Visible=true;
			}
			//Cannot add new patients from OD select patient interface.  Patient must be added from HL7 message.
			if(HL7Defs.IsExistingHL7Enabled()) {
				HL7Def hL7Def=HL7Defs.GetOneDeepEnabled();
				if(hL7Def.ShowDemographics!=HL7ShowDemographics.ChangeAndAdd) {
					_toolBarMain.Buttons["Add"].Enabled=false;
					_toolBarMain.Buttons["Delete"].Enabled=false;
					if(PrefC.GetBool(PrefName.ShowFeaturePatientClone)
						&& _toolBarMain.Buttons["AddClone"]!=null
						&& _toolBarMain.Buttons["SynchClone"]!=null
						&& _toolBarMain.Buttons["BreakClone"]!=null)
					{
						_toolBarMain.Buttons["AddClone"].Enabled=false;
						_toolBarMain.Buttons["SynchClone"].Enabled=false;
						_toolBarMain.Buttons["BreakClone"].Enabled=false;
					}
				}
			}
			else {
				if(Programs.UsingEcwFullMode()) {
					_toolBarMain.Buttons["Add"].Enabled=false;
					_toolBarMain.Buttons["Delete"].Enabled=false;
					if(PrefC.GetBool(PrefName.ShowFeaturePatientClone)
						&& _toolBarMain.Buttons["AddClone"]!=null
						&& _toolBarMain.Buttons["SynchClone"]!=null
						&& _toolBarMain.Buttons["BreakClone"]!=null)
					{
						_toolBarMain.Buttons["AddClone"].Enabled=false;
						_toolBarMain.Buttons["SynchClone"].Enabled=false;
						_toolBarMain.Buttons["BreakClone"].Enabled=false;
					}
				}
			}	
			LayoutManager.LayoutControlBoundsAndFonts(_splitContainerSuperClones);
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
			byte nonConflictingOrdinalNum=1;
			List<PatPlan> listConflictingOrdinalPlans=_listPatPlans.Where(x => x.Ordinal==patPlan.Ordinal && x.PatPlanNum!=patPlan.PatPlanNum).ToList();
			foreach(PatPlan plan in listConflictingOrdinalPlans) {
				//Incrementing until we reach the first unused Ordinal
				while(nonConflictingOrdinalNum<=Byte.MaxValue && _listPatPlans.Count(x => x.Ordinal==nonConflictingOrdinalNum)>0) {
					nonConflictingOrdinalNum++;
					if(nonConflictingOrdinalNum==Byte.MaxValue) {
						break; //This should never happen, but I know that if I don't put this here someone will hit it eventually
					}
				}
				plan.Ordinal=nonConflictingOrdinalNum;
				PatPlans.Update(plan); //This should, in a normal scenario, only run once or twice
			}
		}
		#endregion Methods - Helpers - GridIns

		#region Classes - Nested
		///<summary>Object to hold a dictionary of the most recent completed or EO procedure for each of the ins hist prefs as well as the list of
		///claimprocs for the procedures.  Used to fill gridIns.</summary>
		private class InsProcHist : Tuple<Dictionary<PrefName,Procedure>,List<ClaimProc>> {

			public Dictionary<PrefName,Procedure> DictInsHistProcs => Item1;

			public List<ClaimProc> ListClaimProcs => Item2;

			public InsProcHist(Dictionary<PrefName,Procedure> dictInsHistProcs,List<ClaimProc> listClaimProcs) : base(dictInsHistProcs,listClaimProcs) {
			}
		}
		#endregion Classes - Nested		
	}
}
