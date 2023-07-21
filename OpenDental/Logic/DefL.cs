using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;
using System.Globalization;

namespace OpenDental {
	public class DefL {
		private static string _lanThis="FormDefinitions";
		#region GetMethods
		public static List<DefCatOptions> GetOptionsForDefCats(List<DefCat> listDefCats) {
			List<DefCatOptions> listDefCatOptions = new List<DefCatOptions>();
			for(int i=0;i<listDefCats.Count;i++){
				if(listDefCats[i].GetDescription() == "NotUsed") {
					continue;
				}
				if(listDefCats[i].GetDescription().Contains("HqOnly") && !PrefC.IsODHQ) {
					continue;
				}
				DefCatOptions defCatOptions=new DefCatOptions(listDefCats[i]);
				switch(listDefCats[i]) {
					case DefCat.AccountColors:
						defCatOptions.CanEditName=false;
						defCatOptions.EnableColor=true;
						defCatOptions.HelpText=Lans.g("FormDefinitions","Changes the color of text for different types of entries in Account Module");
						break;
					case DefCat.AccountQuickCharge:
						defCatOptions.CanDelete=true;
						defCatOptions.EnableValue=true;
						defCatOptions.ValueText=Lans.g("FormDefinitions","Procedure Codes");
						defCatOptions.HelpText=Lans.g("FormDefinitions","Account Proc Quick Add items.  Each entry can be a series of procedure codes separated by commas (e.g. D0180,D1101,D8220).  Used in the account module to quickly charge patients for items.");
						break;
					case DefCat.AdjTypes:
						defCatOptions.EnableValue=true;
						defCatOptions.ValueText=Lans.g("FormDefinitions","+, -, or dp");
						defCatOptions.HelpText=Lans.g("FormDefinitions","Plus increases the patient balance.  Minus decreases it.  Dp means discount plan.  Not allowed to change value after creating new type since changes affect all patient accounts.");
						break;
					case DefCat.AppointmentColors:
						defCatOptions.CanEditName=false;
						defCatOptions.EnableColor=true;
						defCatOptions.HelpText=Lans.g("FormDefinitions","Changes colors of background in Appointments Module, and colors for completed appointments.");
						break;
					case DefCat.ApptConfirmed:
						defCatOptions.EnableColor=true;
						defCatOptions.EnableValue=true;
						defCatOptions.ValueText=Lans.g("FormDefinitions","Abbrev");
						defCatOptions.HelpText=Lans.g("FormDefinitions","Color shows on each appointment if Appointment View is set to show ConfirmedColor.");
						break;
					case DefCat.ApptProcsQuickAdd:
						defCatOptions.EnableValue=true;
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
							defCatOptions.ValueText=Lans.g("FormDefinitions","CDA Code(s)");
						}
						else {//USA
							defCatOptions.ValueText=Lans.g("FormDefinitions","ADA Code(s)");
						}
						if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
							defCatOptions.HelpText=Lans.g("FormDefinitions","These are the procedures that you can quickly add to the treatment plan from within the appointment editing window.  Multiple procedures may be separated by commas with no spaces. These definitions may be freely edited without affecting any patient records.");
						}
						else {
							defCatOptions.HelpText=Lans.g("FormDefinitions","These are the procedures that you can quickly add to the treatment plan from within the appointment editing window. Multiple procedures may be separated by commas with no spaces. They generally will not require a tooth number, but a single tooth number is allowed. Example: D1111#8. These definitions may be freely edited without affecting any patient records.");
						}
						break;
					case DefCat.AutoDeposit:
						defCatOptions.CanDelete=true;
						defCatOptions.CanHide=true;
						defCatOptions.EnableValue=true;
						defCatOptions.ValueText=Lans.g("FormDefinitions","Account Number");
						break;
					case DefCat.AutoNoteCats:
						defCatOptions.CanDelete=true;
						defCatOptions.CanHide=false;
						defCatOptions.EnableValue=true;
						defCatOptions.IsValueDefNum=true;
						defCatOptions.ValueText=Lans.g("FormDefinitions","Parent Category");
						defCatOptions.HelpText=Lans.g("FormDefinitions","Each category can have a parent so that categories can be nested. Leave the Parent Category blank for categories at the root level. The order set here will only affect the order within the assigned Parent Category.");
						break;
					case DefCat.BillingTypes:
						defCatOptions.EnableValue=true;
						defCatOptions.ValueText=Lans.g("FormDefinitions","E, C, or CE");
						defCatOptions.HelpText=Lans.g("FormDefinitions","E=Email bill, C=Collection, CE=Collection Excluded.  It is recommended to use as few billing types as possible.  They can be useful when running reports to separate delinquent accounts, but can cause 'forgotten accounts' if used without good office procedures. Changes affect all patients.");
						break;
					case DefCat.BlockoutTypes:
						defCatOptions.EnableColor=true;
						defCatOptions.HelpText=Lans.g("FormDefinitions","Blockout types are used in the appointments module.");
						defCatOptions.EnableValue=true;
						defCatOptions.ValueText=Lans.g("FormDefinitions","Flags");
						break;
					case DefCat.CertificationCategories:
						defCatOptions.HelpText=Lans.g("FormDefinitions","Categories for employee certifications.");
						break;
					case DefCat.ChartGraphicColors:
						defCatOptions.CanEditName=false;
						defCatOptions.EnableColor=true;
						if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
							defCatOptions.HelpText=Lans.g("FormDefinitions","These colors will be used to graphically display treatments.");
						}
						else {
							defCatOptions.HelpText=Lans.g("FormDefinitions","These colors will be used on the graphical tooth chart to draw restorations.");
						}
						break;
					case DefCat.ClaimCustomTracking:
						defCatOptions.CanDelete=true;
						defCatOptions.EnableValue=true;
						defCatOptions.ValueText=Lans.g("FormDefinitions","Days Suppressed");
						defCatOptions.HelpText=Lans.g("FormDefinitions","Some offices may set up claim tracking statuses such as 'review', 'hold', 'riskmanage', etc.")+"\r\n"
							+Lans.g("FormDefinitions","Set the value of 'Days Suppressed' to the number of days the claim will be suppressed from the Outstanding Claims Report "
							+"when the status is changed to the selected status.");
						break;
					case DefCat.ClaimErrorCode:
						defCatOptions.CanDelete=true;
						defCatOptions.CanHide=false;
						defCatOptions.EnableValue=true;
						defCatOptions.ValueText=Lans.g("FormDefinitions","Description");
						defCatOptions.HelpText=Lans.g("FormDefinitions","Used to track error codes when entering claim custom statuses.");
						break;
					case DefCat.ClaimPaymentTracking:
						defCatOptions.ValueText=Lans.g("FormDefinitions","Value");
						defCatOptions.HelpText=Lans.g("FormDefinitions","EOB adjudication method codes to be used for insurance payments.  Last entry cannot be hidden.");
						break;
					case DefCat.ClaimPaymentGroups:
						defCatOptions.ValueText=Lans.g("FormDefinitions","Value");
						defCatOptions.HelpText=Lans.g("FormDefinitions","Used to group claim payments in the daily payments report.");
						break;
					case DefCat.ClinicSpecialty:
						defCatOptions.CanHide=true;
						defCatOptions.CanDelete=false;
						defCatOptions.HelpText=Lans.g("FormDefinitions","You can add as many specialties as you want.  Changes affect all current records.");
						break;
					case DefCat.CommLogTypes:
						defCatOptions.EnableValue=true;
						defCatOptions.EnableColor=true;
						defCatOptions.DoShowNoColor=true;
						string commItemTypes=string.Join(", ",Commlogs.GetCommItemTypes().Select(x => x.GetDescription(useShortVersionIfAvailable:true)));
						defCatOptions.ValueText=Lans.g("FormDefinitions","Usage");
						defCatOptions.HelpText=Lans.g("FormDefinitions","Changes affect all current commlog entries.  Optionally set Usage to one of the following: "
							+commItemTypes+". Only one of each. This helps automate new entries.");
						break;
					case DefCat.ContactCategories:
						defCatOptions.HelpText=Lans.g("FormDefinitions","You can add as many categories as you want.  Changes affect all current contact records.");
						break;
					case DefCat.Diagnosis:
						defCatOptions.EnableValue=true;
						defCatOptions.ValueText=Lans.g("FormDefinitions","1 or 2 letter abbreviation");
						defCatOptions.HelpText=Lans.g("FormDefinitions","The diagnosis list is shown when entering a procedure.  Ones that are less used should go lower on the list.  The abbreviation is shown in the progress notes.  BE VERY CAREFUL.  Changes affect all patients.");
						break;
					case DefCat.EClipboardImageCapture:
						defCatOptions.CanEditName=true;
						defCatOptions.EnableValue=true;
						defCatOptions.EnableColor=false;
						defCatOptions.HelpText=Lans.g("FormDefinitions","Allow patients to capture images in eClipboard");
						defCatOptions.IsValueDefNum=false;
						defCatOptions.DoShowItemOrderInValue=false;
						defCatOptions.DoShowNoColor=false;
						break;
					case DefCat.FeeColors:
						defCatOptions.CanEditName=false;
						defCatOptions.CanHide=false;
						defCatOptions.EnableColor=true;
						defCatOptions.HelpText=Lans.g("FormDefinitions","These are the colors associated to fee types.");
						break;
					case DefCat.ImageCats:
						defCatOptions.ValueText=Lans.g("FormDefinitions","Usage");
						defCatOptions.HelpText=Lans.g("FormDefinitions","These are the categories that will be available in the image and chart modules.  If you hide a category, images in that category will be hidden, so only hide a category if you are certain it has never been used.  Multiple categories can be set to show in the Chart module, but only one category should be set for patient pictures, statements, and tooth charts. Selecting multiple categories for treatment plans will save the treatment plan in each category. Affects all patient records.");
						break;
					case DefCat.InsurancePaymentType:
						defCatOptions.CanDelete=true;
						defCatOptions.CanHide=true;
						defCatOptions.EnableValue=true;
						defCatOptions.ValueText=Lans.g("FormDefinitions","N=Not selected for deposit");
						defCatOptions.HelpText=Lans.g("FormDefinitions","These are claim payment types for insurance payments attached to claims.");
						break;
					case DefCat.InsuranceVerificationStatus:
						defCatOptions.ValueText=Lans.g("FormDefinitions","Usage");
						defCatOptions.HelpText=Lans.g("FormDefinitions","These are statuses for the insurance verification list.");
						break;
					case DefCat.JobPriorities:
						defCatOptions.CanDelete=false;
						defCatOptions.CanHide=true;
						defCatOptions.EnableValue=true;
						defCatOptions.EnableColor=true;
						defCatOptions.ValueText=Lans.g("FormDefinitions","Comma-delimited keywords");
						defCatOptions.HelpText=Lans.g("FormDefinitions","These are job priorities that determine how jobs are sorted in the Job Manager System.  Required values are: OnHold, Low, Normal, MediumHigh, High, Urgent, BugDefault, JobDefault, DocumentationDefault.");
						break;
					case DefCat.LetterMergeCats:
						defCatOptions.HelpText=Lans.g("FormDefinitions","Categories for Letter Merge.  You can safely make any changes you want.");
						break;
					case DefCat.MiscColors:
						defCatOptions.CanEditName=false;
						defCatOptions.EnableColor=true;
						defCatOptions.DoShowNoColor=true;
						defCatOptions.HelpText="";
						break;
					case DefCat.OperatoryTypes:
						defCatOptions.CanDelete=true;
						defCatOptions.CanHide=true;
						defCatOptions.CanEditName=true;
						defCatOptions.HelpText=Lans.g("FormDefinitions","Types for the Operatory. This value is not normally used.");
						break;
					case DefCat.PaymentTypes:
						defCatOptions.EnableValue=true;
						defCatOptions.ValueText=Lans.g("FormDefinitions","N=Not selected for deposit");
						defCatOptions.HelpText=Lans.g("FormDefinitions","Types of payments that patients might make. Any changes will affect all patients.");
						break;
					case DefCat.PayPlanCategories:
						defCatOptions.HelpText=Lans.g("FormDefinitions","Assign payment plans to different categories");
						break;
					case DefCat.PaySplitUnearnedType:
						defCatOptions.ValueText="Do Not Show on Account";
						defCatOptions.HelpText=Lans.g("FormDefinitions","Typically used when a payment is posted to an account with a credit or no balance. Any changes will affect all patients.");
						defCatOptions.EnableValue=true;
						break;
					case DefCat.ProcButtonCats:
						defCatOptions.HelpText=Lans.g("FormDefinitions","These are similar to the procedure code categories, but are only used for organizing and grouping the procedure buttons in the Chart module.");
						break;
					case DefCat.ProcCodeCats:
						defCatOptions.HelpText=Lans.g("FormDefinitions","These are the categories for organizing procedure codes. They do not have to follow ADA categories.  There is no relationship to insurance categories which are setup in the Ins Categories section.  Does not affect any patient records.");
						break;
					case DefCat.ProgNoteColors:
						defCatOptions.CanEditName=false;
						defCatOptions.EnableColor=true;
						defCatOptions.HelpText=Lans.g("FormDefinitions","Changes color of text for different types of entries in the Chart Module Progress Notes.");
						break;
					case DefCat.Prognosis:
						//Nothing special. Might add HelpText later.
						break;
					case DefCat.ProviderSpecialties:
						defCatOptions.HelpText=Lans.g("FormDefinitions","Provider specialties cannot be deleted.  Changes to provider specialties could affect e-claims.");
						break;
					case DefCat.RecallUnschedStatus:
						defCatOptions.EnableValue=true;
						defCatOptions.ValueText=Lans.g("FormDefinitions","Abbreviation");
						defCatOptions.HelpText=Lans.g("FormDefinitions","Recall/Unsched Status.  Abbreviation must be 7 characters or less.  Changes affect all patients.");
						break;
					case DefCat.Regions:
						defCatOptions.CanHide=false;
						defCatOptions.HelpText=Lans.g("FormDefinitions","The region identifying the clinic it is assigned to.");
						break;
					case DefCat.SupplyCats:
						defCatOptions.CanDelete=true;
						defCatOptions.CanHide=false;
						defCatOptions.HelpText=Lans.g("FormDefinitions","The categories for inventory supplies.");
						break;
					case DefCat.TaskCategories:
						defCatOptions.CanDelete=true;
						defCatOptions.DoShowNoColor=true;
						defCatOptions.EnableColor=true;
						defCatOptions.HelpText=Lans.g("FormDefinitions","The categories for tasks. HQ Only as of now.");
						break;
					case DefCat.TaskPriorities:
						defCatOptions.EnableColor=true;
						defCatOptions.EnableValue=true;
						defCatOptions.ValueText=Lans.g("FormDefinitions","D = Default, R = Reminder");
						defCatOptions.HelpText=Lans.g("FormDefinitions","Priorities available for selection within the task edit window.  Task lists are sorted using the order of these priorities.  They can have any description and color.  At least one priority should be Default (D).  If more than one priority is flagged as the default, the last default in the list will be used.  If no default is set, the last priority will be used.  Use (R) to indicate the initial reminder task priority to use when creating reminder tasks.  Changes affect all tasks where the definition is used.");
						break;
					case DefCat.TxPriorities:
						defCatOptions.EnableColor=true;
						defCatOptions.EnableValue=true;
						defCatOptions.DoShowItemOrderInValue=true;
						defCatOptions.ValueText=Lan.g(_lanThis,"Internal Priority");
						defCatOptions.HelpText=Lan.g(_lanThis,"Displayed order should match order of priority of treatment.  They are used in Treatment Plan and Chart "
							+"modules. They can be simple numbers or descriptive abbreviations 7 letters or less.  Changes affect all procedures where the "
							+"definition is used.  'Internal Priority' does not show, but is used for list order and for automated selection of which procedures "
							+"are next in a planned appointment.");
						break;
					case DefCat.WebSchedExistingApptTypes:
						defCatOptions.CanDelete=true;
						defCatOptions.CanHide=false;
						defCatOptions.ValueText="Appointment Type";
						defCatOptions.HelpText="Appointment types to be displayed in the Web Sched Existing Patient web application.  These are selectable by patients and will be saved to the appointment note.";
						break;
					case DefCat.WebSchedNewPatApptTypes:
						defCatOptions.CanDelete=true;
						defCatOptions.CanHide=false;
						defCatOptions.ValueText=Lans.g("FormDefinitions","Appointment Type");
						defCatOptions.HelpText=Lans.g("FormDefinitions","Appointment types to be displayed in the Web Sched New Pat Appt web application.  These are selectable for the new patients and will be saved to the appointment note.");
						break;
					case DefCat.CarrierGroupNames:
						defCatOptions.CanHide=true;
						defCatOptions.HelpText=Lans.g("FormDefinitions","These are group names for Carriers.");
						break;
					case DefCat.TimeCardAdjTypes:
						defCatOptions.CanEditName=true;
						defCatOptions.CanHide=true;
						defCatOptions.HelpText=Lans.g("FormDefinitions","These are PTO Adjustments Types used for tracking on employee time cards and ADP export.");
						break;
				}
				listDefCatOptions.Add(defCatOptions);
			}
			return listDefCatOptions;
		}

		private static string GetItemDescForImages(string itemValue) {
			List<string> listDescriptions=new List<string>();
			if(itemValue.Contains("X")) {
				listDescriptions.Add(Lan.g(_lanThis,"ChartModule"));
			}
			if(itemValue.Contains("M")) {
				listDescriptions.Add(Lan.g(_lanThis,"Thumbnails"));
			}
			if(itemValue.Contains("F")) {
				listDescriptions.Add(Lan.g(_lanThis,"PatientForm"));
			}
			if(itemValue.Contains("P")){
				listDescriptions.Add(Lan.g(_lanThis,"PatientPic"));
			}
			if(itemValue.Contains("S")){
				listDescriptions.Add(Lan.g(_lanThis,"Statement"));
			}
			if(itemValue.Contains("T")){
				listDescriptions.Add(Lan.g(_lanThis,"ToothChart"));
			}
			if(itemValue.Contains("R")) {
				listDescriptions.Add(Lan.g(_lanThis,"TreatPlans"));
			}
			if(itemValue.Contains("L")) {
				listDescriptions.Add(Lan.g(_lanThis,"PatientPortal"));
			}
			if(itemValue.Contains("A")) {
				listDescriptions.Add(Lan.g(_lanThis,"PayPlans"));
			}
			if(itemValue.Contains("C")) {
				listDescriptions.Add(Lan.g(_lanThis,"ClaimAttachments"));
			}
			if(itemValue.Contains("B")) {
				listDescriptions.Add(Lan.g(_lanThis,"LabCases"));
			}
			if(itemValue.Contains("U")) {
				listDescriptions.Add(Lan.g(_lanThis,"AutoSaveForms"));
			}
			if(itemValue.Contains("Y")) {
				listDescriptions.Add(Lan.g(_lanThis,"TaskAttachments"));
			}
			if(itemValue.Contains("N")) {
				listDescriptions.Add(Lan.g(_lanThis,"ClaimResponses"));
			}
			return string.Join(", ",listDescriptions);
		}
		#endregion
		///<summary>Fills the passed in grid with the definitions in the passed in list.</summary>
		public static void FillGridDefs(GridOD gridDefs,DefCatOptions defCatOptionsSelected,List<Def> listDefs) {
			Def defSelected=null;
			if(gridDefs.GetSelectedIndex() > -1) {
				defSelected=(Def)gridDefs.ListGridRows[gridDefs.GetSelectedIndex()].Tag;
			}
			int scroll=gridDefs.ScrollValue;
			gridDefs.BeginUpdate();
			gridDefs.Columns.Clear();
			GridColumn col;
			col = new GridColumn(Lan.g("TableDefs","Name"),190);
			gridDefs.Columns.Add(col);
			col = new GridColumn(defCatOptionsSelected.ValueText,190);
			gridDefs.Columns.Add(col);
			col = new GridColumn(defCatOptionsSelected.EnableColor ? Lan.g("TableDefs","Color") : "",40);
			gridDefs.Columns.Add(col);
			col = new GridColumn(defCatOptionsSelected.CanHide ? Lan.g("TableDefs","Hide") : "",30,HorizontalAlignment.Center);
			gridDefs.Columns.Add(col);
			gridDefs.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listDefs.Count;i++){
				if(!PrefC.IsODHQ && listDefs[i].ItemValue==CommItemTypeAuto.ODHQ.ToString()) {
					continue;
				}
				if(Defs.IsDefDeprecated(listDefs[i])) {
					listDefs[i].IsHidden=true;
				}
				row=new GridRow();
				if(defCatOptionsSelected.CanEditName) {
					row.Cells.Add(listDefs[i].ItemName);
				}
				else {//Users cannot edit the item name so let them translate them.
					row.Cells.Add(Lan.g("FormDefinitions",listDefs[i].ItemName));//Doesn't use 'this' so that renaming the form doesn't change the translation
				}
				if(defCatOptionsSelected.DefCat==DefCat.ImageCats) {
					row.Cells.Add(GetItemDescForImages(listDefs[i].ItemValue));
				}
				else if(defCatOptionsSelected.DefCat==DefCat.AutoNoteCats) {
					Dictionary<string,string> dictAutoNoteDefs = new Dictionary<string,string>();
					dictAutoNoteDefs=listDefs.ToDictionary(x => x.DefNum.ToString(),x => x.ItemName);
					string nameCur;
					row.Cells.Add(dictAutoNoteDefs.TryGetValue(listDefs[i].ItemValue,out nameCur) ? nameCur : listDefs[i].ItemValue);
				}
				else if(defCatOptionsSelected.DefCat==DefCat.WebSchedNewPatApptTypes || defCatOptionsSelected.DefCat==DefCat.WebSchedExistingApptTypes) {
					AppointmentType appointmentType=AppointmentTypes.GetApptTypeForDef(listDefs[i].DefNum);
					row.Cells.Add(appointmentType==null ? "" : appointmentType.AppointmentTypeName);
				}
				else if(defCatOptionsSelected.DoShowItemOrderInValue) {
					row.Cells.Add(listDefs[i].ItemOrder.ToString());
				}
				else {
					row.Cells.Add(listDefs[i].ItemValue);
				}
				row.Cells.Add("");
				if(defCatOptionsSelected.EnableColor) {
					row.Cells[row.Cells.Count-1].ColorBackG=listDefs[i].ItemColor;
				}
				if(listDefs[i].IsHidden) {
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}
				row.Tag=listDefs[i];
				gridDefs.ListGridRows.Add(row);
			}
			gridDefs.EndUpdate();
			if(defSelected!=null) {
				for(int i=0;i < gridDefs.ListGridRows.Count;i++) {
					if(((Def)gridDefs.ListGridRows[i].Tag).DefNum == defSelected.DefNum) {
						gridDefs.SetSelected(i,true);
						break;
					}
				}
			}
			gridDefs.ScrollValue=scroll;
		}

		public static bool GridDefsDoubleClick(Def defSelected,GridOD gridDefs,DefCatOptions defCatOptionsSelected,List<Def> listDefs,List<Def> listDefsAll,bool isDefChanged) {
			switch(defCatOptionsSelected.DefCat) {
				case DefCat.BlockoutTypes:
					using(FormDefEditBlockout formDefEditBlockout=new FormDefEditBlockout(defSelected)) {
						formDefEditBlockout.ShowDialog();
						if(formDefEditBlockout.DialogResult==DialogResult.OK) {
							isDefChanged=true;
						}
					}
					break;
				case DefCat.ImageCats:
					using(FormDefEditImages formDefEditImages=new FormDefEditImages(defSelected)) {
						formDefEditImages.IsNew=false;
						formDefEditImages.ShowDialog();
						if(formDefEditImages.DialogResult==DialogResult.OK) {
							isDefChanged=true;
						}
					}
					break;
				case DefCat.WebSchedExistingApptTypes:
					using(FormDefEditWebSchedApptTypes formDefEditWebSchedApptTypes=new FormDefEditWebSchedApptTypes(defSelected,"Edit Web Sched Existing Patient Appt Type")) {
						if(formDefEditWebSchedApptTypes.ShowDialog()==DialogResult.OK) {
							if(formDefEditWebSchedApptTypes.IsDeleted) {
								listDefsAll.Remove(defSelected);
							}
							isDefChanged=true;
						}
					}
					break;
				case DefCat.WebSchedNewPatApptTypes:
					using(FormDefEditWebSchedApptTypes formDefEditWebSchedApptTypes=new FormDefEditWebSchedApptTypes(defSelected,"Edit Web Sched New Patient Appt Type")) {
						if(formDefEditWebSchedApptTypes.ShowDialog()==DialogResult.OK) {
							if(formDefEditWebSchedApptTypes.IsDeleted) {
								listDefsAll.Remove(defSelected);
							}
							isDefChanged=true;
						}
					}
					break;
				default://Show the normal FormDefEdit window.
					using(FormDefEdit FormDefEdit=new FormDefEdit(defSelected,listDefs,defCatOptionsSelected)) {
						FormDefEdit.IsNew=false;
						FormDefEdit.ShowDialog();
						if(FormDefEdit.DialogResult==DialogResult.OK) {
							if(FormDefEdit.IsDeleted) {
								listDefsAll.Remove(defSelected);
							}
							isDefChanged=true;
						}
					}
					break;
			}
			return isDefChanged;
		}

		public static bool AddDef(GridOD gridDefs,DefCatOptions defCatOptionsSelected) {
			Def def=new Def();
			def.IsNew=true;
			int itemOrder=0;
			if(Defs.GetDefsForCategory(defCatOptionsSelected.DefCat).Count>0) {
				itemOrder=Defs.GetDefsForCategory(defCatOptionsSelected.DefCat).Max(x => x.ItemOrder) + 1;
			}
			def.ItemOrder=itemOrder;
			def.Category=defCatOptionsSelected.DefCat;
			def.ItemName="";
			def.ItemValue="";//necessary
			if(defCatOptionsSelected.DefCat==DefCat.InsurancePaymentType) {
				def.ItemValue="N";
			}
			switch(defCatOptionsSelected.DefCat) {
				case DefCat.BlockoutTypes:
					using(FormDefEditBlockout formDefEditBlockout=new FormDefEditBlockout(def)) {
						formDefEditBlockout.IsNew=true;
						if(formDefEditBlockout.ShowDialog()!=DialogResult.OK) {
							return false;
						}
					}
					break;
				case DefCat.ImageCats:
					using(FormDefEditImages formDefEditImages=new FormDefEditImages(def)) {
						formDefEditImages.IsNew=true;
						formDefEditImages.ShowDialog();
						if(formDefEditImages.DialogResult!=DialogResult.OK) {
							return false;
						}
					}
					break;
				case DefCat.WebSchedExistingApptTypes:
					using(FormDefEditWebSchedApptTypes formDefEditWebSchedApptTypes=new FormDefEditWebSchedApptTypes(def,"Edit Web Sched Existing Patient Appt Type")) {
						if(formDefEditWebSchedApptTypes.ShowDialog()!=DialogResult.OK) {
							return false;
						}
					}
					break;
				case DefCat.WebSchedNewPatApptTypes:
					using(FormDefEditWebSchedApptTypes formDefEditWebSchedApptTypes=new FormDefEditWebSchedApptTypes(def,"Edit Web Sched New Patient Appt Type")) {
						if(formDefEditWebSchedApptTypes.ShowDialog()!=DialogResult.OK) { 
							return false;
						}
					}
					break;
				default:
					List<Def> listDefsCurrent=new List<Def>();
					for(int i=0;i<gridDefs.ListGridRows.Count;i++){
						listDefsCurrent.Add((Def)gridDefs.ListGridRows[i].Tag);
					}
					using(FormDefEdit formDefEdit=new FormDefEdit(def,listDefsCurrent,defCatOptionsSelected)) {
						formDefEdit.IsNew=true;
						formDefEdit.ShowDialog();
						if(formDefEdit.DialogResult!=DialogResult.OK) {
							return false;
						}
					}
					break;
			}
			return true;
		}

		///<summary>Will attempt to hide the currently selected definition of the ODGrid that is passed in.</summary>
		public static bool TryHideDefSelectedInGrid(GridOD gridDefs,DefCatOptions selectedDefCatOpt) {
			if(gridDefs.GetSelectedIndex()==-1) {
				MsgBox.Show(_lanThis,"Please select item first,");
				return false;
			}
			Def defSelected=(Def)gridDefs.ListGridRows[gridDefs.GetSelectedIndex()].Tag;
			if(!CanHideDef(defSelected,selectedDefCatOpt)) {
				return false;
			}
			DefL.HideDef(defSelected);
			return true;
		}

		///<summary>Returns true if definition can be hidden or is already hidden. Displays error message and returns false if not.</summary>
		public static bool CanHideDef(Def def,DefCatOptions defCatOptions) {
			if(def.IsHidden) {//Return true if Def is already hidden.
				return true;
			}
			if(!defCatOptions.CanHide || !defCatOptions.CanEditName) {
				MsgBox.Show(_lanThis,"Definitions of this category cannot be hidden.");
				return false;//We should never get here, but if we do, something went wrong because the definition shouldn't have been hideable
			}
			//Stop users from hiding the last definition in categories that must have at least one def in them.
			List<Def> listDefsNotHidden=Defs.GetDefsForCategory(defCatOptions.DefCat,true);
			if(Defs.NeedOneUnhidden(def.Category) && listDefsNotHidden.Count==1) {
				MsgBox.Show(_lanThis,"You cannot hide the last definition in this category.");
				return false;
			}
			if(def.Category==DefCat.ProviderSpecialties){
				if(Providers.IsSpecialtyInUse(def.DefNum)){
					MsgBox.Show(_lanThis,"You cannot hide a specialty if it is in use by a provider.");
					return false;
				}
				if(Referrals.IsSpecialtyInUse(def.DefNum)){
					MsgBox.Show(_lanThis,"You cannot hide a specialty if it is in use by a referral source.");
					return false;
				}
			}
			if(Defs.IsDefinitionInUse(def)) {//DefNum will be zero if it is being created but hasn't been saved to DB yet, thus it can't be in use.
				bool isClinicDefaultBillingType=ClinicPrefs.GetPrefAllClinics(PrefName.PracticeDefaultBillType).Any(x => x.ValueString==def.DefNum.ToString());
				if(def.DefNum.In(
					PrefC.GetLong(PrefName.BrokenAppointmentAdjustmentType),
					PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger),
					PrefC.GetLong(PrefName.AppointmentTimeSeatedTrigger),
					PrefC.GetLong(PrefName.AppointmentTimeDismissedTrigger),
					PrefC.GetLong(PrefName.TreatPlanDiscountAdjustmentType),
					PrefC.GetLong(PrefName.BillingChargeAdjustmentType),
					PrefC.GetLong(PrefName.FinanceChargeAdjustmentType),
					PrefC.GetLong(PrefName.LateChargeAdjustmentType),
					PrefC.GetLong(PrefName.PrepaymentUnearnedType),
					PrefC.GetLong(PrefName.SalesTaxAdjustmentType),
					PrefC.GetLong(PrefName.RecurringChargesPayTypeCC),
					PrefC.GetLong(PrefName.RefundAdjustmentType)))
					//PrefC.GetLong(PrefName.TpUnearnedType))) //We can hide this because of the combo box code which will still set a default
				{
					MsgBox.Show(_lanThis,"You cannot hide a definition if it is in use within Module Preferences.");
					return false;
				}
				else if(def.DefNum.In(
					PrefC.GetLong(PrefName.RecallStatusMailed),
					PrefC.GetLong(PrefName.RecallStatusTexted),
					PrefC.GetLong(PrefName.RecallStatusEmailed),
					PrefC.GetLong(PrefName.RecallStatusEmailedTexted)))
				{
					MsgBox.Show(_lanThis,"You cannot hide a definition that is used as a status in the Setup Recall window.");
					return false;
				}
				else if(def.DefNum==PrefC.GetLong(PrefName.WebSchedNewPatConfirmStatus)) {
					MsgBox.Show(_lanThis,"You cannot hide a definition that is used as an appointment confirmation status in Web Sched New Pat Appt.");
					return false;
				}
				else if(def.DefNum==PrefC.GetLong(PrefName.WebSchedRecallConfirmStatus)) {
					MsgBox.Show(_lanThis,"You cannot hide a definition that is used as an appointment confirmation status in Web Sched Recall Appt.");
					return false;
				}
				else if(def.DefNum==PrefC.GetLong(PrefName.PracticeDefaultBillType)) {
					MsgBox.Show(_lanThis,"You cannot hide a billing type when it is selected as the practice default billing type.");
					return false;
				}
				else if(isClinicDefaultBillingType) {
					MsgBox.Show(_lanThis,"You cannot hide a billing type when it is selected as a clinic's default billing type.");
					return false;
				}
				else if(Defs.IsPaymentTypeInUse(def)) {
					MsgBox.Show(_lanThis,"You cannot hide a payment type when it is the default payment type for PayConnect, PaySimple, EdgeExpress, or XCharge.");
					return false;
				}
				else {
					if(!MsgBox.Show(_lanThis,MsgBoxButtons.OKCancel,"Warning: This definition is currently in use within the program.")) {
						return false;
					}
				}
			}
			if(def.Category==DefCat.PaySplitUnearnedType) {
				if(listDefsNotHidden.FindAll(x => string.IsNullOrEmpty(x.ItemValue)).Count==1 && def.ItemValue=="") {
					MsgBox.Show(_lanThis,"Must have at least one definition that shows in Account");
					return false;
				}
			}
			//Warn the user if they are about to hide a billing type currently in use.
			if(defCatOptions.DefCat==DefCat.BillingTypes && Patients.IsBillingTypeInUse(def.DefNum)) {
				if(!MsgBox.Show(_lanThis,MsgBoxButtons.OKCancel,
					"Warning: Billing type is currently in use by patients, insurance plans, or preferences.")) 
				{
					return false;
				}
			}
			if(def.Category==DefCat.EClipboardImageCapture && EClipboardImageCaptureDefs.IsEClipboardImageDefInUse(def.DefNum)) {
				MsgBox.Show(_lanThis,"You cannot hide an eClipboard Image Capture definition that is in use in 'eClipboard Setup'.");
					return false;
			}
			return true;
		}

		public static bool UpClick(GridOD gridDefs) {
			if(gridDefs.GetSelectedIndex()==-1){
				MessageBox.Show(Lan.g("Defs","Please select an item first."));
				return false;
			}
			if(gridDefs.GetSelectedIndex()==0) {
				return false;
			}
			Def defSelected=(Def)gridDefs.ListGridRows[gridDefs.GetSelectedIndex()].Tag;
			Def defAbove=(Def)gridDefs.ListGridRows[gridDefs.GetSelectedIndex()-1].Tag;
			int indexDefSelectedItemOrder=defSelected.ItemOrder;
			defSelected.ItemOrder=defAbove.ItemOrder;
			defAbove.ItemOrder=indexDefSelectedItemOrder;
			DefL.Update(defSelected);
			DefL.Update(defAbove);
			return true;
		}

		public static bool DownClick(GridOD gridDefs) {
			if(gridDefs.GetSelectedIndex()==-1){
				MessageBox.Show(Lan.g("Defs","Please select an item first."));
				return false;
			}
			if(gridDefs.GetSelectedIndex()==gridDefs.ListGridRows.Count-1) {
				return false;
			}
			Def defSelected=(Def)gridDefs.ListGridRows[gridDefs.GetSelectedIndex()].Tag;
			Def defBelow=(Def)gridDefs.ListGridRows[gridDefs.GetSelectedIndex()+1].Tag;
			int indexDefSelectedItemOrder=defSelected.ItemOrder;
			defSelected.ItemOrder=defBelow.ItemOrder;
			defBelow.ItemOrder=indexDefSelectedItemOrder;
			DefL.Update(defSelected);
			DefL.Update(defBelow);
			return true;
		}

		///<summary>Also handles a security log entry.</summary>
		public static long Insert(Def def) {
			string logText=Lan.g("Defintions","Definition created:")+" "+def.ItemName+" "
				+Lan.g("Defintions","with category:")+" "+def.Category.GetDescription();
			SecurityLogs.MakeLogEntry(Permissions.DefEdit,0,logText);
			return Defs.Insert(def);
		}

		///<summary>Also handles a security log entry.</summary>
		public static void Update(Def def) {
			string logText=Lan.g("Defintions","Definition edited:")+" "+def.ItemName+" "
				+Lan.g("Defintions","with category:")+" "+def.Category.GetDescription();
			SecurityLogs.MakeLogEntry(Permissions.DefEdit,0,logText);
			Defs.Update(def);
		}

		///<summary>Also handles a security log entry.</summary>
		public static void HideDef(Def def) {
			string logText=Lan.g("Defintions","Definition hidden:")+" "+def.ItemName+" "
				+Lan.g("Defintions","with category:")+" "+def.Category.GetDescription();
			SecurityLogs.MakeLogEntry(Permissions.DefEdit,0,logText);
			Defs.HideDef(def);
		}
	}
}
