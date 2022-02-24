using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.WebTypes.WebSched.TimeSlot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormEServicesWebSchedPat:FormODBase {
		#region Fields
		///<summary>Keeps track of the last selected index for the Web Sched Appt URL grid.</summary>
		private int _indexLastClinic=-1;
		///<summary>Set to true whenever the thread is already running while another thing wants it to refresh yet again.
		///E.g. The window loads which initially starts a fill thread and then the user quickly starts changing filters.</summary>
		private bool _isApptTimeSlotsOutdated=false;
		///<summary>Determines which preference set we should use (new or existing patients).</summary>
		private bool _isNewPat;
		///<summary>Prevents TextChanged from firing a thread on load.</summary>
		private bool _isLoading=true;
		private ODThread _threadFillGridApptTimeSlots=null;
		private DefCat _defCatApptTypes;
		private PrefName _prefNameAllowProvSelection;
		private PrefName _prefNameApptMessage;
		private PrefName _prefNameConfirmStatus;
		private PrefName _prefNamePreventDoubleBooking;
		private PrefName _prefNameIgnoreBlockoutTypes;
		private PrefName _prefNameSearchAfterDays;
		///<summary>Used to get IsEnabled for each clinic when filling out gridApptTimeSlots.</summary>
		public WebServiceMainHQProxy.EServiceSetup.SignupOut SignupOut=null;
		#endregion Fields

		public FormEServicesWebSchedPat(bool isNewPat=false) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_isNewPat=isNewPat;
			this.Text="Web Sched - Existing Patient";
			if(_isNewPat) {
				this.Text="Web Sched - New Patient";
			}
		}

		private void FormEServicesWebSchedPat_Load(object sender,EventArgs e) {
			if(SignupOut==null) {
				SignupOut=FormEServicesSetup.GetSignupOut();
			}
			SetPreferences();
			bool allowEdit=Security.IsAuthorized(Permissions.EServicesSetup,true);
			Authorize(allowEdit);
			int searchAfterDays=PrefC.GetInt(_prefNameSearchAfterDays);
			textApptMessage.Text=PrefC.GetString(_prefNameApptMessage);
			textApptSearchDays.Text=searchAfterDays>0 ? searchAfterDays.ToString() : "";
			string apptType=_isNewPat ? "New Patient" : "Existing Patient";
			labelNoApptType.Text="Could not load timeslots because no "+apptType+" Appointment Type was found";
			checkNewPatForcePhoneFormatting.Checked=PrefC.GetBool(PrefName.WebSchedNewPatApptForcePhoneFormatting);
			checkNewPatForcePhoneFormatting.Visible=_isNewPat;
			DateTime dateSearch=DateTime.Now;
			dateSearch=dateSearch.AddDays(searchAfterDays);
			textApptsDateStart.Text=dateSearch.ToShortDateString();
			FillGridReasons();
			FillComboClinics();
			FillBlockoutTypes();
			FillGridApptOps();
			//This needs to happen after all of the previous fills because it's asynchronous.
			FillGridApptTimeSlotsThreaded();
			long defaultStatus=PrefC.GetLong(_prefNameConfirmStatus);
			comboConfirmStatuses.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ApptConfirmed,true));
			comboConfirmStatuses.SetSelectedDefNum(defaultStatus);
			checkAllowProvSelection.Checked=PrefC.GetBool(_prefNameAllowProvSelection);
			if(!PrefC.HasClinicsEnabled) {
				labelClinic.Visible=false;
				comboClinics.Visible=false;
				labelTimeSlots.Visible=false;
			}
			checkDoubleBooking.Checked=PrefC.GetInt(_prefNamePreventDoubleBooking)>0;//0 = Allow double booking, 1 = prevent
			_isLoading=false;
		}

		#region Methods - Public
		/// <summary>Saves either the NewPat or ExistingPat preferences, depending on which mode the control was created.</summary>
		public void SaveWebSchedPat() {
			Prefs.UpdateString(_prefNameApptMessage,textApptMessage.Text);
			Prefs.UpdateBool(_prefNameAllowProvSelection,checkAllowProvSelection.Checked);
			Prefs.UpdateInt(_prefNamePreventDoubleBooking,checkDoubleBooking.Checked ? 1 : 0);
			if(comboConfirmStatuses.SelectedIndex!=-1) {
				Prefs.UpdateLong(_prefNameConfirmStatus,comboConfirmStatuses.GetSelectedDefNum());
			}
		}
		#endregion Methods - Public

		#region Methods - Private
		private void Authorize(bool allowEdit) {
			butWebSchedBlockouts.Enabled=allowEdit;
			textApptSearchDays.Enabled=allowEdit;
		}

		/// <summary>Fills comboClinics with the related signup information per clinic.</summary>
		private void FillComboClinics() {
			List<Clinic> clinicsAll=Clinics.GetDeepCopy();
			var eServiceData=WebServiceMainHQProxy.GetSignups<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService>(SignupOut,eServiceCode.WebSchedNewPatAppt)
				.Select(x => new {
					Signup=x,
					ClinicName=(clinicsAll.FirstOrDefault(y => y.ClinicNum==x.ClinicNum)??new Clinic() { Abbr="N\\A" }).Abbr
				})
				.Where(x => 
					//When clinics off, only show headquarters
					(!PrefC.HasClinicsEnabled && x.Signup.ClinicNum==0) || 
					//When clinics are on, only show if not hidden.
					(PrefC.HasClinicsEnabled && clinicsAll.Any(y => y.ClinicNum==x.Signup.ClinicNum && !y.IsHidden))
				)
				//Alpha sorted
				.OrderBy(x => x.ClinicName);
			foreach(var clinic in eServiceData) {
				comboClinics.Items.Add(clinic.ClinicName,clinic.Signup);
			}
		}

		private void FillGridApptOps() {
			int opNameWidth=150;
			int clinicWidth=150;
			if(!PrefC.HasClinicsEnabled) {
				opNameWidth+=clinicWidth;
			}
			gridApptOps.BeginUpdate();
			gridApptOps.ListGridColumns.Clear();
			gridApptOps.ListGridColumns.Add(new GridColumn(Lan.g("TableOperatories","Op Name"),opNameWidth));
			gridApptOps.ListGridColumns.Add(new GridColumn(Lan.g("TableOperatories","Abbrev"),60));
			if(PrefC.HasClinicsEnabled) {
				gridApptOps.ListGridColumns.Add(new GridColumn(Lan.g("TableOperatories","Clinic"),clinicWidth));
			}
			gridApptOps.ListGridColumns.Add(new GridColumn(Lan.g("TableOperatories","Provider"),60));
			gridApptOps.ListGridColumns.Add(new GridColumn(Lan.g("TableOperatories","Hygienist"),60));
			gridApptOps.ListGridColumns.Add(new GridColumn(Lan.g("TableOperatories","ApptTypes"),40) { IsWidthDynamic=true });
			gridApptOps.ListGridRows.Clear();
			//A list of all operatories that are considered for web sched new / existing pat appt.
			List<Operatory> listOps=Operatories.GetOpsForWebSchedNewOrExistingPatAppts(_isNewPat);
			List<long> listDefNums;
			if(_isNewPat) {
				listDefNums=listOps.SelectMany(x => x.ListWSNPAOperatoryDefNums).Distinct().ToList();
			}
			else {
				listDefNums=listOps.SelectMany(x => x.ListWSEPOperatoryDefNums).Distinct().ToList();
			}
			List<Def> listDefs=Defs.GetDefs(_defCatApptTypes,listDefNums);
			GridRow row;
			foreach(Operatory op in listOps) {
				row=new GridRow();
				row.Cells.Add(op.OpName);
				row.Cells.Add(op.Abbrev);
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(op.ClinicNum));
				}
				row.Cells.Add(Providers.GetAbbr(op.ProvDentist));
				row.Cells.Add(Providers.GetAbbr(op.ProvHygienist));
				//Display the name of all "appointment types" (definition.ItemName) that are associated with the current operatory.
				if(_isNewPat) {
					row.Cells.Add(string.Join(", ",listDefs.Where(x => op.ListWSNPAOperatoryDefNums.Any(y => y==x.DefNum)).Select(x => x.ItemName)));
				}
				else {
					row.Cells.Add(string.Join(", ",listDefs.Where(x => op.ListWSEPOperatoryDefNums.Any(y => y==x.DefNum)).Select(x => x.ItemName)));
				}
				row.Tag=op;
				gridApptOps.ListGridRows.Add(row);
			}
			gridApptOps.EndUpdate();
		}

		private void FillGridApptTimeSlots(List<TimeSlot> listTimeSlots) {
			if(this.InvokeRequired) {
				this.Invoke((Action)delegate () { FillGridApptTimeSlots(listTimeSlots); });
				return;
			}
			gridApptTimeSlots.BeginUpdate();
			gridApptTimeSlots.ListGridColumns.Clear();
			GridColumn col=new GridColumn("",20){ IsWidthDynamic=true };
			col.TextAlign=HorizontalAlignment.Center;
			gridApptTimeSlots.ListGridColumns.Add(col);
			gridApptTimeSlots.ListGridRows.Clear();
			GridRow row;
			DateTime dateTimeSlotLast=DateTime.MinValue;
			foreach(TimeSlot timeSlot in listTimeSlots) {
				//Make a new row for every unique day.
				if(dateTimeSlotLast.Date!=timeSlot.DateTimeStart.Date) {
					dateTimeSlotLast=timeSlot.DateTimeStart;
					row=new GridRow();
					row.ColorBackG=Color.LightBlue;
					row.Cells.Add(timeSlot.DateTimeStart.ToShortDateString());
					gridApptTimeSlots.ListGridRows.Add(row);
				}
				row=new GridRow();
				row.Cells.Add(timeSlot.DateTimeStart.ToShortTimeString()+" - "+timeSlot.DateTimeStop.ToShortTimeString());
				gridApptTimeSlots.ListGridRows.Add(row);
			}
			gridApptTimeSlots.EndUpdate();
		}

		private void FillGridApptTimeSlotsThreaded() {
			if(this.InvokeRequired) {
				this.BeginInvoke((Action)delegate () {
					FillGridApptTimeSlotsThreaded();
				});
				return;
			}
			if(comboDefApptType.GetSelected<Def>()==null) {
				labelNoApptType.Visible=true;
				return;
			}
			else {
				labelNoApptType.Visible=false;
			}
			//Clear the current grid rows before starting the thread below. This allows that thread to exit at any time without leaving old rows in the grid.
			gridApptTimeSlots.BeginUpdate();
			gridApptTimeSlots.ListGridRows.Clear();
			gridApptTimeSlots.EndUpdate();
			//Validate time slot settings.
			if(!textApptsDateStart.IsValid()) {
				//Don't bother warning the user.  It will just be annoying.  The red indicator should be sufficient.
				return;
			}
			if(!PrefC.HasClinicsEnabled) {
				comboClinics.SelectedIndex=0;//Not visible but this will set the combo box the "N/A" which is the non-clinic signup
			}
			if(comboClinics.SelectedIndex<0) {
				return;//Nothing to do.
			}
			WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService signup=comboClinics.GetSelected<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService>();
			//Protect against re-entry
			if(_threadFillGridApptTimeSlots!=null) {
				//A thread is already refreshing the time slots grid so we simply need to queue up another refresh once the one thread has finished.
				_isApptTimeSlotsOutdated=true;
				return;
			}
			_isApptTimeSlotsOutdated=false;
			_indexLastClinic=comboClinics.SelectedIndex;
			DateTime dateStart=PIn.DateT(textApptsDateStart.Text);
			DateTime dateEnd=dateStart.AddDays(30);
			if(!signup.IsEnabled) {
				return;//Do nothing, this clinic is excluded.
			}
			//Only get time slots for headquarters or clinics that are NOT excluded (aka included).
			var args=new {
				ClinicNum=signup.ClinicNum,
				DateStart=dateStart,
				DateEnd=dateStart.AddDays(30),
				DefApptType=comboDefApptType.GetSelected<Def>(),
			};
			_threadFillGridApptTimeSlots=new ODThread(new ODThread.WorkerDelegate((th) => {
				//The user might not have Web Sched ops set up correctly.  Don't warn them here because it is just annoying.  They'll figure it out.
				ODException.SwallowAnyException(() => {
					//Get the next 30 days of open time schedules with the current settings
					List<TimeSlot> listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(args.DateStart,args.DateEnd,args.ClinicNum
						,args.DefApptType.DefNum,_isNewPat);
					FillGridApptTimeSlots(listTimeSlots);
				});
			})) { Name="ThreadApptTimeSlots" };
			_threadFillGridApptTimeSlots.AddExitHandler(new ODThread.WorkerDelegate((th) => {
				_threadFillGridApptTimeSlots=null;
				//If something else wanted to refresh the grid while we were busy filling it then we need to refresh again.  A filter could have changed.
				if(_isApptTimeSlotsOutdated) {
					FillGridApptTimeSlotsThreaded();
				}
			}));
			_threadFillGridApptTimeSlots.Start(true);
		}

		private void FillBlockoutTypes() {
			//Get all WSNPA / WSEPA reasons (defs)
			List<Def> listReasonDefs=Defs.GetDefsForCategory(_defCatApptTypes,isShort:true);
			//Get every blockout type deflink for the reason defs.
			List<DefLink> listRestrictedToDefLinks=DefLinks.GetDefLinksByTypeAndDefs(DefLinkType.BlockoutType,listReasonDefs.Select(x => x.DefNum).ToList());
			List<long> listRestrictedToDefNums=listRestrictedToDefLinks.Select(x => x.FKey).Distinct().ToList();
			List<Def> listRestrictedToBlockoutDefs=Defs.GetDefs(DefCat.BlockoutTypes,listRestrictedToDefNums);
			List<long> listAllowedBlockoutTypes=GetAllowedBlockoutTypes();
			List<Def> listAllowedBlockoutDefs=Defs.GetDefs(DefCat.BlockoutTypes,listAllowedBlockoutTypes.FindAll(x => !ListTools.In(x,listRestrictedToDefNums)));
			//Fill the list box for Restricted to Reasons that will display the blockout name followed by associated reason names.
			List<string> listBlockoutReasonAssociations=new List<string>();
			foreach(Def blockout in listRestrictedToBlockoutDefs) {
				//Find the reasons for this restricted to blockout which are FKey def links.
				List<long> listAssociatedReasonNums=listRestrictedToDefLinks.Where(x => x.FKey==blockout.DefNum).Select(x => x.DefNum).ToList();
				List<string> listAssociatedReasonNames=listReasonDefs.Where(x => ListTools.In(x.DefNum,listAssociatedReasonNums)).Select(x => x.ItemName).ToList();
				listBlockoutReasonAssociations.Add(blockout.ItemName + " ("+string.Join(",",listAssociatedReasonNames)+")");
			}
			listboxRestrictedToReasons.Items.Clear();
			listboxRestrictedToReasons.Items.AddList(listBlockoutReasonAssociations, x => x);
			//Fill the list box for Generally Allowed
			listboxIgnoreBlockoutTypes.Items.Clear();
			listboxIgnoreBlockoutTypes.Items.AddList(listAllowedBlockoutDefs.Select(x => x.ItemName), x => x);
		}

		private void FillGridReasons() {
			List<Def> listDefs=Defs.GetDefsForCategory(_defCatApptTypes,true);
			List<DefLink> listDefLinks=DefLinks.GetDefLinksByType(DefLinkType.AppointmentType);
			List<AppointmentType> listApptTypes=AppointmentTypes.GetWhere(x => listDefLinks.Any(y => y.FKey==x.AppointmentTypeNum),true);
			//The combo box within the available times group box should always reflect the grid.
			comboDefApptType.Items.Clear();
			gridReasons.BeginUpdate();
			gridReasons.ListGridColumns.Clear();
			gridReasons.ListGridColumns.Add(new GridColumn(Lan.g(this,"Reason"),250));
			gridReasons.ListGridColumns.Add(new GridColumn(Lan.g(this,"Pattern"),80) { IsWidthDynamic=true });
			gridReasons.ListGridRows.Clear();
			GridRow row;
			foreach(Def def in listDefs) {
				AppointmentType apptType=null;
				DefLink defLink=listDefLinks.FirstOrDefault(x => x.DefNum==def.DefNum);
				if(defLink==null) {
					continue;//Corruption?
				}
				apptType=listApptTypes.FirstOrDefault(x => x.AppointmentTypeNum==defLink.FKey);
				if(apptType==null) {
					continue;//Corruption?
				}
				row=new GridRow();
				row.Cells.Add(def.ItemName);
				row.Cells.Add((string.IsNullOrEmpty(apptType.Pattern) ? Lan.g(this,"(use procedure time pattern)") : Appointments.ConvertPatternFrom5(apptType.Pattern)));
				gridReasons.ListGridRows.Add(row);
				comboDefApptType.Items.Add(def.ItemName,def);
			}
			gridReasons.EndUpdate();
			if(comboDefApptType.Items.Count > 0) {
				comboDefApptType.SelectedIndex=0;//Select Default.
			}
		}

		/// <summary>Fetches either NewPat or ExistingPat allowed blockout types, depending on which mode the control was created.</summary>
		private List<long> GetAllowedBlockoutTypes() {
			if(_isNewPat) {
				return PrefC.GetWebSchedNewPatAllowedBlockouts;
			}
			else {
				return PrefC.GetWebSchedExistingPatAllowedBlockouts;
			}
		}

		private void SetPreferences() {
			if(_isNewPat) {
				_defCatApptTypes=DefCat.WebSchedNewPatApptTypes;
				_prefNameAllowProvSelection=PrefName.WebSchedNewPatAllowProvSelection;
				_prefNameApptMessage=PrefName.WebSchedNewPatApptMessage;
				_prefNameConfirmStatus=PrefName.WebSchedNewPatConfirmStatus;
				_prefNamePreventDoubleBooking=PrefName.WebSchedNewPatApptDoubleBooking;
				_prefNameIgnoreBlockoutTypes=PrefName.WebSchedNewPatApptIgnoreBlockoutTypes;
				_prefNameSearchAfterDays=PrefName.WebSchedNewPatApptSearchAfterDays;
			}
			else {
				_defCatApptTypes=DefCat.WebSchedExistingApptTypes;
				_prefNameAllowProvSelection=PrefName.WebSchedExistingPatAllowProvSelection;
				_prefNameApptMessage=PrefName.WebSchedExistingPatMessage;
				_prefNameConfirmStatus=PrefName.WebSchedExistingPatConfirmStatus;
				_prefNamePreventDoubleBooking=PrefName.WebSchedExistingPatDoubleBooking;
				_prefNameIgnoreBlockoutTypes=PrefName.WebSchedExistingPatIgnoreBlockoutTypes;
				_prefNameSearchAfterDays=PrefName.WebSchedExistingPatSearchAfterDays;
			}
		}

		///<summary>Shows the Operatories window and allows the user to edit them.  Does not show the window if user does not have Setup permission.
		///Refreshes all corresponding grids within the Web Sched tab that display Operatory information.  Feel free to add to this method.</summary>
		private void ShowOperatoryEditAndRefreshGrids() {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormOperatories formOperatories=new FormOperatories();
			formOperatories.ShowDialog();
			if(formOperatories.ListConflictingAppts.Count>0) {
				FormApptConflicts formApptConflicts=new FormApptConflicts(formOperatories.ListConflictingAppts);
				formApptConflicts.Show();
				formApptConflicts.BringToFront();
			}
			FillGridApptOps();
			FillGridApptTimeSlotsThreaded();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Operatories accessed via EServices Setup window.");
		}
		#endregion Methods - Private

		#region Methods - Event Handlers
		private void butApptsToday_Click(object sender,EventArgs e) {
			textApptsDateStart.Text=DateTime.Today.ToShortDateString();
			textApptsDateStart.Validate();//clears the (!) if necessary.
		}

		private void butBlockouts_Click(object sender,EventArgs e) {
			List<long> listBlockoutTypes=GetAllowedBlockoutTypes();
			List<Def> listBlockoutTypeDefs=Defs.GetDefs(DefCat.BlockoutTypes,listBlockoutTypes);
			using FormDefinitionPicker formDefinitionPicker=new FormDefinitionPicker(DefCat.BlockoutTypes,listBlockoutTypeDefs);
			formDefinitionPicker.HasShowHiddenOption=true;
			formDefinitionPicker.IsMultiSelectionMode=true;
			formDefinitionPicker.ShowDialog();
			if(formDefinitionPicker.DialogResult==DialogResult.OK) {
				string strListIgnoreBlockoutTypes=String.Join(",",formDefinitionPicker.ListSelectedDefs.Select(x => x.DefNum));
				Prefs.UpdateString(_prefNameIgnoreBlockoutTypes,strListIgnoreBlockoutTypes);
				FillBlockoutTypes();
			}
		}

		private void butNotify_Click(object sender,EventArgs e) {
			WebSchedNotifyType webSchedNotifyType=_isNewPat ? WebSchedNotifyType.NewPat : WebSchedNotifyType.ExistingPat;
			using FormEServicesWebSchedNotify formEServicesWebSchedNotify=new FormEServicesWebSchedNotify(webSchedNotifyType);
			formEServicesWebSchedNotify.ShowDialog();
		}

		private void butRestrictedToReasonsEdit_Click(object sender,EventArgs e) {
			using FormDefinitions formDefinitions=new FormDefinitions(_defCatApptTypes);
			formDefinitions.ShowDialog();
			FillBlockoutTypes();
		}

		private void butOK_Click(object sender,EventArgs e) {
			SaveWebSchedPat();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void checkWebSchedNewPatForcePhoneFormatting_Click(object sender,EventArgs e) {
			Prefs.UpdateBool(PrefName.WebSchedNewPatApptForcePhoneFormatting,checkNewPatForcePhoneFormatting.Checked);
		}

		private void comboApptType_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGridApptTimeSlotsThreaded();
		}

		private void comboClinics_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboClinics.SelectedIndex!=_indexLastClinic) {
				FillGridApptTimeSlotsThreaded();
			}
		}

		private void gridApptOps_DoubleClick(object sender,EventArgs e) {
			ShowOperatoryEditAndRefreshGrids();
		}

		private void gridReasons_DoubleClick(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormDefinitions formDefinitions=new FormDefinitions(_defCatApptTypes);
			formDefinitions.ShowDialog();
			FillGridReasons();
			FillBlockoutTypes();
			FillGridApptTimeSlotsThreaded();
		}

		private void textApptSearchDays_Leave(object sender,EventArgs e) {
			//Only refresh if the value of this preference changed. _indexLastClinic will be set to -1 if a refresh is needed.
			if(_indexLastClinic==-1) {
				FillGridApptTimeSlotsThreaded();
			}
		}

		private void textApptSearchDays_Validated(object sender,EventArgs e) {
			if(!textApptSearchDays.IsValid()) {
				return;
			}
			int apptDays=PIn.Int(textApptSearchDays.Text);
			if(Prefs.UpdateInt(_prefNameSearchAfterDays,apptDays>0 ? apptDays : 0)) {
				_indexLastClinic=-1;//Force refresh of the grid in because this setting changed.
			}
		}

		private void textApptsDateStart_TextChanged(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			//Only refresh the grid if the user has typed in a valid date.
			if(textApptsDateStart.IsValid()) {
				FillGridApptTimeSlotsThreaded();
			}
		}
		#endregion Methods - Event Handlers
	}
}