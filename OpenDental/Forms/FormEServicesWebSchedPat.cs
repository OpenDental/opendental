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
		///<summary>Determines which preference set we should use (new or existing patients).</summary>
		private bool _isNewPat;
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
			butWebSchedBlockouts.Enabled=allowEdit;
			textApptSearchDays.Enabled=allowEdit;
			int searchAfterDays=PrefC.GetInt(_prefNameSearchAfterDays);
			textApptMessage.Text=PrefC.GetString(_prefNameApptMessage);
			textApptSearchDays.Text="";
			if(searchAfterDays>0) {
				textApptSearchDays.Text=searchAfterDays.ToString();
			}
			string appointmentType="Existing Patient";
			if(_isNewPat) {
				appointmentType="New Patient";
			}
			labelNoApptType.Text="Could not load timeslots because no "+appointmentType+" Appointment Type was found";
			checkNewPatForcePhoneFormatting.Checked=PrefC.GetBool(PrefName.WebSchedNewPatApptForcePhoneFormatting);
			checkNewPatForcePhoneFormatting.Visible=_isNewPat;
			DateTime dateSearch=DateTime.Now;
			dateSearch=dateSearch.AddDays(searchAfterDays);
			textApptsDateStart.Text=dateSearch.ToShortDateString();
			FillGridReasons();
			FillComboClinics();
			FillBlockoutTypes();
			FillGridApptOps();
			FillGridApptTimeSlots();//only fills grid if no clinics.
			long defaultStatus=PrefC.GetLong(_prefNameConfirmStatus);
			comboConfirmStatuses.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ApptConfirmed,true));
			comboConfirmStatuses.SetSelectedDefNum(defaultStatus);
			checkAllowProvSelection.Checked=PrefC.GetBool(_prefNameAllowProvSelection);
			if(!PrefC.HasClinicsEnabled) {
				labelClinic.Visible=false;
				comboClinics.Visible=false;
			}
			checkDoubleBooking.Checked=PrefC.GetInt(_prefNamePreventDoubleBooking)>0;//0 = Allow double booking, 1 = prevent
		}

		#region Methods - Public
		/// <summary>Saves either the NewPat or ExistingPat preferences, depending on which mode the control was created.</summary>
		public void SaveWebSchedPat() {
			Prefs.UpdateString(_prefNameApptMessage,textApptMessage.Text);
			Prefs.UpdateBool(_prefNameAllowProvSelection,checkAllowProvSelection.Checked);
			if(checkDoubleBooking.Checked) {
				Prefs.UpdateInt(_prefNamePreventDoubleBooking,1);
			}
			else {
				Prefs.UpdateInt(_prefNamePreventDoubleBooking,0);
			}
			if(comboConfirmStatuses.SelectedIndex!=-1) {
				Prefs.UpdateLong(_prefNameConfirmStatus,comboConfirmStatuses.GetSelectedDefNum());
			}
		}
		#endregion Methods - Public

		#region Methods - Private
		private class EServiceClinicName {
			public WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService Signup;
			public string ClinicName;
		}

		/// <summary>Fills comboClinics with the related signup information per clinic.</summary>
		private void FillComboClinics() {
			List<Clinic> listAllClinics=Clinics.GetDeepCopy();
			List<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService> listSignupOutEServices=WebServiceMainHQProxy.GetSignups<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService>(SignupOut,eServiceCode.WebSchedNewPatAppt);
			List<EServiceClinicName> listEServiceClinicName=new List<EServiceClinicName>();
			Clinic clinic;
			for(int i=0;i<listSignupOutEServices.Count;i++) {
				if(PrefC.HasClinicsEnabled){
					if(!listAllClinics.Exists(x => x.ClinicNum==listSignupOutEServices[i].ClinicNum)) {
						continue;
					}
					if(listAllClinics.Find(x => x.ClinicNum==listSignupOutEServices[i].ClinicNum).IsHidden) {
						continue;
					}
				}
				else {//clinics off
					if(listSignupOutEServices[i].ClinicNum!=0) {//only show headquarters
						continue;
					}
				}
				EServiceClinicName eServiceClinicName=new EServiceClinicName();
				clinic=listAllClinics.FirstOrDefault(x => x.ClinicNum==listSignupOutEServices[i].ClinicNum);
				if(clinic==null) {
					eServiceClinicName.ClinicName="N\\A";
				}
				else {
					eServiceClinicName.ClinicName=clinic.Abbr;
				}
				eServiceClinicName.Signup=listSignupOutEServices[i];
				listEServiceClinicName.Add(eServiceClinicName);
			}
			listEServiceClinicName=listEServiceClinicName.OrderBy(x => x.ClinicName).ToList();
			for(int i=0;i<listEServiceClinicName.Count();i++) {
				comboClinics.Items.Add(listEServiceClinicName[i].ClinicName,listEServiceClinicName[i].Signup);
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
			for(int i=0;i<listOps.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listOps[i].OpName);
				row.Cells.Add(listOps[i].Abbrev);
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(listOps[i].ClinicNum));
				}
				row.Cells.Add(Providers.GetAbbr(listOps[i].ProvDentist));
				row.Cells.Add(Providers.GetAbbr(listOps[i].ProvHygienist));
				//Display the name of all "appointment types" (definition.ItemName) that are associated with the current operatory.
				if(_isNewPat) {
					row.Cells.Add(string.Join(", ",listDefs.Where(x => listOps[i].ListWSNPAOperatoryDefNums.Any(y => y==x.DefNum)).Select(x => x.ItemName)));
				}
				else {
					row.Cells.Add(string.Join(", ",listDefs.Where(x => listOps[i].ListWSEPOperatoryDefNums.Any(y => y==x.DefNum)).Select(x => x.ItemName)));
				}
				row.Tag=listOps[i];
				gridApptOps.ListGridRows.Add(row);
			}
			gridApptOps.EndUpdate();
		}

		private void FillGridApptTimeSlots() {
			if(comboDefApptType.GetSelected<Def>()==null) {
				labelNoApptType.Visible=true;
				return;
			}
			else {
				labelNoApptType.Visible=false;
			}
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
			if(!signup.IsEnabled) {
				return;//Do nothing, this clinic is excluded.
			}
			DateTime dateStart=PIn.DateT(textApptsDateStart.Text);
			DateTime dateEnd=dateStart.AddDays(30);
			gridApptTimeSlots.BeginUpdate();
			List<TimeSlot> listTimeSlots=new List<TimeSlot>();
			//This throws exceptions when the selected clinic does not have a operatory with it.
			//Takes a few minutes with the nadg database.
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(dateStart,dateStart.AddDays(30),signup.ClinicNum,comboDefApptType.GetSelected<Def>().DefNum,_isNewPat);
			};
			progressOD.StartingMessage=Lan.g(this,"Loading available time slots. This can take a few minutes on large databases.");
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				MsgBox.Show(ex.Message);
			}
			if(progressOD.IsCancelled){
				//nothing to do.
			}
			gridApptTimeSlots.ListGridColumns.Clear();
			GridColumn col=new GridColumn("",20){ IsWidthDynamic=true };
			col.TextAlign=HorizontalAlignment.Center;
			gridApptTimeSlots.ListGridColumns.Add(col);
			gridApptTimeSlots.ListGridRows.Clear();
			GridRow row;
			DateTime dateTimeSlotLast=DateTime.MinValue;
			for(int i=0;i<listTimeSlots.Count;i++) {
				//Make a new row for every unique day.
				if(dateTimeSlotLast.Date!=listTimeSlots[i].DateTimeStart.Date) {
					dateTimeSlotLast=listTimeSlots[i].DateTimeStart;
					row=new GridRow();
					row.ColorBackG=Color.LightBlue;
					row.Cells.Add(listTimeSlots[i].DateTimeStart.ToShortDateString());
					gridApptTimeSlots.ListGridRows.Add(row);
				}
				row=new GridRow();
				row.Cells.Add(listTimeSlots[i].DateTimeStart.ToShortTimeString()+" - "+listTimeSlots[i].DateTimeStop.ToShortTimeString());
				gridApptTimeSlots.ListGridRows.Add(row);
			}
			gridApptTimeSlots.EndUpdate();
		}

		private void FillBlockoutTypes() {
			//Get all WSNPA / WSEPA reasons (defs)
			List<Def> listDefReasons=Defs.GetDefsForCategory(_defCatApptTypes,isShort:true);
			//Get every blockout type deflink for the reason defs.
			List<DefLink> listDefLinkRestrictedTo=DefLinks.GetDefLinksByTypeAndDefs(DefLinkType.BlockoutType,listDefReasons.Select(x => x.DefNum).ToList());
			List<long> listRestrictedToDefNums=listDefLinkRestrictedTo.Select(x => x.FKey).Distinct().ToList();
			List<Def> listDefRestrictedToBlockouts=Defs.GetDefs(DefCat.BlockoutTypes,listRestrictedToDefNums);
			List<long> listAllowedBlockoutTypes=GetAllowedBlockoutTypes();
			List<Def> listDefAllowedBlockouts=Defs.GetDefs(DefCat.BlockoutTypes,listAllowedBlockoutTypes.FindAll(x => !ListTools.In(x,listRestrictedToDefNums)));
			//Fill the list box for Restricted to Reasons that will display the blockout name followed by associated reason names.
			List<string> listBlockoutReasonAssociations=new List<string>();
			List<long> listAssociatedReasonNums=new List<long>();
			List<string> listAssociatedReasonNames=new List<string>();
			for(int i=0;i<listDefRestrictedToBlockouts.Count;i++) {
				//Find the reasons for this restricted to blockout which are FKey def links.
				listAssociatedReasonNums=listDefLinkRestrictedTo.Where(x => x.FKey==listDefRestrictedToBlockouts[i].DefNum).Select(x => x.DefNum).ToList();
				listAssociatedReasonNames=listDefReasons.Where(x => ListTools.In(x.DefNum,listAssociatedReasonNums)).Select(x => x.ItemName).ToList();
				listBlockoutReasonAssociations.Add(listDefRestrictedToBlockouts[i].ItemName + " ("+string.Join(",",listAssociatedReasonNames)+")");
			}
			listboxRestrictedToReasons.Items.Clear();
			listboxRestrictedToReasons.Items.AddList(listBlockoutReasonAssociations, x => x);
			//Fill the list box for Generally Allowed
			listboxIgnoreBlockoutTypes.Items.Clear();
			listboxIgnoreBlockoutTypes.Items.AddList(listDefAllowedBlockouts.Select(x => x.ItemName), x => x);
		}

		private void FillGridReasons() {
			List<Def> listDefs=Defs.GetDefsForCategory(_defCatApptTypes,true);
			List<DefLink> listDefLinks=DefLinks.GetDefLinksByType(DefLinkType.AppointmentType);
			List<AppointmentType> listAppointmentTypes=AppointmentTypes.GetWhere(x => listDefLinks.Any(y => y.FKey==x.AppointmentTypeNum),true);
			//The combo box within the available times group box should always reflect the grid.
			comboDefApptType.Items.Clear();
			gridReasons.BeginUpdate();
			gridReasons.ListGridColumns.Clear();
			gridReasons.ListGridColumns.Add(new GridColumn(Lan.g(this,"Reason"),250));
			gridReasons.ListGridColumns.Add(new GridColumn(Lan.g(this,"Pattern"),80) { IsWidthDynamic=true });
			gridReasons.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listDefs.Count;i++) {
				AppointmentType appointmentType=null;
				DefLink defLink=listDefLinks.FirstOrDefault(x => x.DefNum==listDefs[i].DefNum);
				if(defLink==null) {
					continue;//Corruption?
				}
				appointmentType=listAppointmentTypes.FirstOrDefault(x => x.AppointmentTypeNum==defLink.FKey);
				if(appointmentType==null) {
					continue;//Corruption?
				}
				row=new GridRow();
				row.Cells.Add(listDefs[i].ItemName);
				if(string.IsNullOrEmpty(appointmentType.Pattern)) {
					row.Cells.Add(Lan.g(this,"(use procedure time pattern)"));
				}
				else {
					row.Cells.Add(Appointments.ConvertPatternFrom5(appointmentType.Pattern));
				}
				gridReasons.ListGridRows.Add(row);
				comboDefApptType.Items.Add(listDefs[i].ItemName,listDefs[i]);
			}
			gridReasons.EndUpdate();
			if(comboDefApptType.Items.Count>0) {
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
		#endregion Methods - Private

		#region Methods - Event Handlers
		private void butApptsToday_Click(object sender,EventArgs e) {
			textApptsDateStart.Text=DateTime.Today.ToShortDateString();
			textApptsDateStart.Validate();//clears the (!) if necessary.
		}

		private void butBlockouts_Click(object sender,EventArgs e) {
			List<long> listBlockoutTypes=GetAllowedBlockoutTypes();
			List<Def> listDefBlockoutTypes=Defs.GetDefs(DefCat.BlockoutTypes,listBlockoutTypes);
			using FormDefinitionPicker formDefinitionPicker=new FormDefinitionPicker(DefCat.BlockoutTypes,listDefBlockoutTypes);
			formDefinitionPicker.HasShowHiddenOption=true;
			formDefinitionPicker.IsMultiSelectionMode=true;
			formDefinitionPicker.ShowDialog();
			if(formDefinitionPicker.DialogResult==DialogResult.OK) {
				string strListIgnoreBlockoutTypes=String.Join(",",formDefinitionPicker.ListDefsSelected.Select(x => x.DefNum));
				Prefs.UpdateString(_prefNameIgnoreBlockoutTypes,strListIgnoreBlockoutTypes);
				FillBlockoutTypes();
			}
		}

		private void butNotify_Click(object sender,EventArgs e) {
			WebSchedNotifyType webSchedNotifyType=WebSchedNotifyType.ExistingPat;
			if(_isNewPat) {
				webSchedNotifyType=WebSchedNotifyType.NewPat;
			}
			using FormEServicesWebSchedNotify formEServicesWebSchedNotify=new FormEServicesWebSchedNotify(webSchedNotifyType);
			formEServicesWebSchedNotify.ShowDialog();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGridApptTimeSlots();
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

		private void butEditOps_Click(object sender,EventArgs e) {
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
			FillGridApptTimeSlots();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Operatories accessed via EServices Setup window.");
		}

		private void butEditReasons_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormDefinitions formDefinitions=new FormDefinitions(_defCatApptTypes);
			formDefinitions.ShowDialog();
			FillGridReasons();
			FillBlockoutTypes();
			FillGridApptTimeSlots();
		}

		private void textApptSearchDays_Validated(object sender,EventArgs e) {
			if(!textApptSearchDays.IsValid()) {
				return;
			}
			int appointmentDays=PIn.Int(textApptSearchDays.Text);
			if(appointmentDays<=0) {
				appointmentDays=0;
			}
			Prefs.UpdateInt(_prefNameSearchAfterDays,appointmentDays);
		}
		#endregion Methods - Event Handlers
	}
}