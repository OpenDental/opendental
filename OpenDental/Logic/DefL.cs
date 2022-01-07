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
		public static List<DefCatOptions> GetOptionsForDefCats(Array defCatVals) {
			List<DefCatOptions> listDefCatOptions = new List<DefCatOptions>();
			foreach(DefCat defCatCur in defCatVals) {
				if(defCatCur.GetDescription() == "NotUsed") {
					continue;
				}
				if(defCatCur.GetDescription().Contains("HqOnly") && !PrefC.IsODHQ) {
					continue;
				}
				DefCatOptions defCOption=new DefCatOptions(defCatCur);
				switch(defCatCur) {
					case DefCat.AccountColors:
						defCOption.CanEditName=false;
						defCOption.EnableColor=true;
						defCOption.HelpText=Lans.g("FormDefinitions","Changes the color of text for different types of entries in Account Module");
						break;
					case DefCat.AccountQuickCharge:
						defCOption.CanDelete=true;
						defCOption.EnableValue=true;
						defCOption.ValueText=Lans.g("FormDefinitions","Procedure Codes");
						defCOption.HelpText=Lans.g("FormDefinitions","Account Proc Quick Add items.  Each entry can be a series of procedure codes separated by commas (e.g. D0180,D1101,D8220).  Used in the account module to quickly charge patients for items.");
						break;
					case DefCat.AdjTypes:
						defCOption.EnableValue=true;
						defCOption.ValueText=Lans.g("FormDefinitions","+, -, or dp");
						defCOption.HelpText=Lans.g("FormDefinitions","Plus increases the patient balance.  Minus decreases it.  Dp means discount plan.  Not allowed to change value after creating new type since changes affect all patient accounts.");
						break;
					case DefCat.AppointmentColors:
						defCOption.CanEditName=false;
						defCOption.EnableColor=true;
						defCOption.HelpText=Lans.g("FormDefinitions","Changes colors of background in Appointments Module, and colors for completed appointments.");
						break;
					case DefCat.ApptConfirmed:
						defCOption.EnableColor=true;
						defCOption.EnableValue=true;
						defCOption.ValueText=Lans.g("FormDefinitions","Abbrev");
						defCOption.HelpText=Lans.g("FormDefinitions","Color shows on each appointment if Appointment View is set to show ConfirmedColor.");
						break;
					case DefCat.ApptProcsQuickAdd:
						defCOption.EnableValue=true;
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
							defCOption.ValueText=Lans.g("FormDefinitions","CDA Code(s)");
						}
						else {//USA
							defCOption.ValueText=Lans.g("FormDefinitions","ADA Code(s)");
						}
						if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
							defCOption.HelpText=Lans.g("FormDefinitions","These are the procedures that you can quickly add to the treatment plan from within the appointment editing window.  Multiple procedures may be separated by commas with no spaces. These definitions may be freely edited without affecting any patient records.");
						}
						else {
							defCOption.HelpText=Lans.g("FormDefinitions","These are the procedures that you can quickly add to the treatment plan from within the appointment editing window.  They must not require a tooth number. Multiple procedures may be separated by commas with no spaces. These definitions may be freely edited without affecting any patient records.");
						}
						break;
					case DefCat.AutoDeposit:
						defCOption.CanDelete=true;
						defCOption.CanHide=true;
						defCOption.EnableValue=true;
						defCOption.ValueText=Lans.g("FormDefinitions","Account Number");
						break;
					case DefCat.AutoNoteCats:
						defCOption.CanDelete=true;
						defCOption.CanHide=false;
						defCOption.EnableValue=true;
						defCOption.IsValueDefNum=true;
						defCOption.ValueText=Lans.g("FormDefinitions","Parent Category");
						defCOption.HelpText=Lans.g("FormDefinitions","Leave the Parent Category blank for categories at the root level. Assign a Parent Category to move a category within another. The order set here will only affect the order within the assigned Parent Category in the Auto Note list. For example, a category may be moved above its parent in this list, but it will still be within its Parent Category in the Auto Note list.");
						break;
					case DefCat.BillingTypes:
						defCOption.EnableValue=true;
						defCOption.ValueText=Lans.g("FormDefinitions","E, C, or CE");
						defCOption.HelpText=Lans.g("FormDefinitions","E=Email bill, C=Collection, CE=Collection Excluded.  It is recommended to use as few billing types as possible.  They can be useful when running reports to separate delinquent accounts, but can cause 'forgotten accounts' if used without good office procedures. Changes affect all patients.");
						break;
					case DefCat.BlockoutTypes:
						defCOption.EnableColor=true;
						defCOption.HelpText=Lans.g("FormDefinitions","Blockout types are used in the appointments module.");
						defCOption.EnableValue=true;
						defCOption.ValueText=Lans.g("FormDefinitions","Flags");
						break;
					case DefCat.CertificationCategories:
						defCOption.HelpText=Lans.g("FormDefinitions","Categories for employee certifications.");
						break;
					case DefCat.ChartGraphicColors:
						defCOption.CanEditName=false;
						defCOption.EnableColor=true;
						if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
							defCOption.HelpText=Lans.g("FormDefinitions","These colors will be used to graphically display treatments.");
						}
						else {
							defCOption.HelpText=Lans.g("FormDefinitions","These colors will be used on the graphical tooth chart to draw restorations.");
						}
						break;
					case DefCat.ClaimCustomTracking:
						defCOption.CanDelete=true;
						defCOption.EnableValue=true;
						defCOption.ValueText=Lans.g("FormDefinitions","Days Suppressed");
						defCOption.HelpText=Lans.g("FormDefinitions","Some offices may set up claim tracking statuses such as 'review', 'hold', 'riskmanage', etc.")+"\r\n"
							+Lans.g("FormDefinitions","Set the value of 'Days Suppressed' to the number of days the claim will be suppressed from the Outstanding Claims Report "
							+"when the status is changed to the selected status.");
						break;
					case DefCat.ClaimErrorCode:
						defCOption.CanDelete=true;
						defCOption.CanHide=false;
						defCOption.EnableValue=true;
						defCOption.ValueText=Lans.g("FormDefinitions","Description");
						defCOption.HelpText=Lans.g("FormDefinitions","Used to track error codes when entering claim custom statuses.");
						break;
					case DefCat.ClaimPaymentTracking:
						defCOption.ValueText=Lans.g("FormDefinitions","Value");
						defCOption.HelpText=Lans.g("FormDefinitions","EOB adjudication method codes to be used for insurance payments.  Last entry cannot be hidden.");
						break;
					case DefCat.ClaimPaymentGroups:
						defCOption.ValueText=Lans.g("FormDefinitions","Value");
						defCOption.HelpText=Lans.g("FormDefinitions","Used to group claim payments in the daily payments report.");
						break;
					case DefCat.ClinicSpecialty:
						defCOption.CanHide=true;
						defCOption.CanDelete=false;
						defCOption.HelpText=Lans.g("FormDefinitions","You can add as many specialties as you want.  Changes affect all current records.");
						break;
					case DefCat.CommLogTypes:
						defCOption.EnableValue=true;
						defCOption.EnableColor=true;
						defCOption.DoShowNoColor=true;
						string commItemTypes=string.Join(", ",Commlogs.GetCommItemTypes().Select(x => x.GetDescription(useShortVersionIfAvailable:true)));
						defCOption.ValueText=Lans.g("FormDefinitions","Usage");
						defCOption.HelpText=Lans.g("FormDefinitions","Changes affect all current commlog entries.  Optionally set Usage to one of the following: "
							+commItemTypes+". Only one of each. This helps automate new entries.");
						break;
					case DefCat.ContactCategories:
						defCOption.HelpText=Lans.g("FormDefinitions","You can add as many categories as you want.  Changes affect all current contact records.");
						break;
					case DefCat.Diagnosis:
						defCOption.EnableValue=true;
						defCOption.ValueText=Lans.g("FormDefinitions","1 or 2 letter abbreviation");
						defCOption.HelpText=Lans.g("FormDefinitions","The diagnosis list is shown when entering a procedure.  Ones that are less used should go lower on the list.  The abbreviation is shown in the progress notes.  BE VERY CAREFUL.  Changes affect all patients.");
						break;
					case DefCat.EClipboardImageCapture:
						defCOption.CanEditName=true;
						defCOption.EnableValue=true;
						defCOption.EnableColor=false;
						defCOption.HelpText=Lans.g("FormDefinitions","Allow patients to capture images in eClipboard");
						defCOption.IsValueDefNum=false;
						defCOption.DoShowItemOrderInValue=false;
						defCOption.DoShowNoColor=false;
						break;
					case DefCat.FeeColors:
						defCOption.CanEditName=false;
						defCOption.CanHide=false;
						defCOption.EnableColor=true;
						defCOption.HelpText=Lans.g("FormDefinitions","These are the colors associated to fee types.");
						break;
					case DefCat.ImageCats:
						defCOption.ValueText=Lans.g("FormDefinitions","Usage");
						defCOption.HelpText=Lans.g("FormDefinitions","These are the categories that will be available in the image and chart modules.  If you hide a category, images in that category will be hidden, so only hide a category if you are certain it has never been used.  Multiple categories can be set to show in the Chart module, but only one category should be set for patient pictures, statements, and tooth charts. Selecting multiple categories for treatment plans will save the treatment plan in each category. Affects all patient records.");
						break;
					case DefCat.InsurancePaymentType:
						defCOption.CanDelete=true;
						defCOption.CanHide=false;
						defCOption.EnableValue=true;
						defCOption.ValueText=Lans.g("FormDefinitions","N=Not selected for deposit");
						defCOption.HelpText=Lans.g("FormDefinitions","These are claim payment types for insurance payments attached to claims.");
						break;
					case DefCat.InsuranceVerificationStatus:
						defCOption.ValueText=Lans.g("FormDefinitions","Usage");
						defCOption.HelpText=Lans.g("FormDefinitions","These are statuses for the insurance verification list.");
						break;
					case DefCat.JobPriorities:
						defCOption.CanDelete=false;
						defCOption.CanHide=true;
						defCOption.EnableValue=true;
						defCOption.EnableColor=true;
						defCOption.ValueText=Lans.g("FormDefinitions","Comma-delimited keywords");
						defCOption.HelpText=Lans.g("FormDefinitions","These are job priorities that determine how jobs are sorted in the Job Manager System.  Required values are: OnHold, Low, Normal, MediumHigh, High, Urgent, BugDefault, JobDefault, DocumentationDefault.");
						break;
					case DefCat.LetterMergeCats:
						defCOption.HelpText=Lans.g("FormDefinitions","Categories for Letter Merge.  You can safely make any changes you want.");
						break;
					case DefCat.MiscColors:
						defCOption.CanEditName=false;
						defCOption.EnableColor=true;
						defCOption.DoShowNoColor=true;
						defCOption.HelpText="";
						break;
					case DefCat.PaymentTypes:
						defCOption.EnableValue=true;
						defCOption.ValueText=Lans.g("FormDefinitions","N=Not selected for deposit");
						defCOption.HelpText=Lans.g("FormDefinitions","Types of payments that patients might make. Any changes will affect all patients.");
						break;
					case DefCat.PayPlanCategories:
						defCOption.HelpText=Lans.g("FormDefinitions","Assign payment plans to different categories");
						break;
					case DefCat.PaySplitUnearnedType:
						defCOption.ValueText="Do Not Show on Account";
						defCOption.HelpText=Lans.g("FormDefinitions","Typically used when a payment is posted to an account with a credit or no balance. Any changes will affect all patients.");
						defCOption.EnableValue=true;
						break;
					case DefCat.ProcButtonCats:
						defCOption.HelpText=Lans.g("FormDefinitions","These are similar to the procedure code categories, but are only used for organizing and grouping the procedure buttons in the Chart module.");
						break;
					case DefCat.ProcCodeCats:
						defCOption.HelpText=Lans.g("FormDefinitions","These are the categories for organizing procedure codes. They do not have to follow ADA categories.  There is no relationship to insurance categories which are setup in the Ins Categories section.  Does not affect any patient records.");
						break;
					case DefCat.ProgNoteColors:
						defCOption.CanEditName=false;
						defCOption.EnableColor=true;
						defCOption.HelpText=Lans.g("FormDefinitions","Changes color of text for different types of entries in the Chart Module Progress Notes.");
						break;
					case DefCat.Prognosis:
						//Nothing special. Might add HelpText later.
						break;
					case DefCat.ProviderSpecialties:
						defCOption.HelpText=Lans.g("FormDefinitions","Provider specialties cannot be deleted.  Changes to provider specialties could affect e-claims.");
						break;
					case DefCat.RecallUnschedStatus:
						defCOption.EnableValue=true;
						defCOption.ValueText=Lans.g("FormDefinitions","Abbreviation");
						defCOption.HelpText=Lans.g("FormDefinitions","Recall/Unsched Status.  Abbreviation must be 7 characters or less.  Changes affect all patients.");
						break;
					case DefCat.Regions:
						defCOption.CanHide=false;
						defCOption.HelpText=Lans.g("FormDefinitions","The region identifying the clinic it is assigned to.");
						break;
					case DefCat.SupplyCats:
						defCOption.CanDelete=true;
						defCOption.CanHide=false;
						defCOption.HelpText=Lans.g("FormDefinitions","The categories for inventory supplies.");
						break;
					case DefCat.TaskPriorities:
						defCOption.EnableColor=true;
						defCOption.EnableValue=true;
						defCOption.ValueText=Lans.g("FormDefinitions","D = Default, R = Reminder");
						defCOption.HelpText=Lans.g("FormDefinitions","Priorities available for selection within the task edit window.  Task lists are sorted using the order of these priorities.  They can have any description and color.  At least one priority should be Default (D).  If more than one priority is flagged as the default, the last default in the list will be used.  If no default is set, the last priority will be used.  Use (R) to indicate the initial reminder task priority to use when creating reminder tasks.  Changes affect all tasks where the definition is used.");
						break;
					case DefCat.TxPriorities:
						defCOption.EnableColor=true;
						defCOption.EnableValue=true;
						defCOption.DoShowItemOrderInValue=true;
						defCOption.ValueText=Lan.g(_lanThis,"Internal Priority");
						defCOption.HelpText=Lan.g(_lanThis,"Displayed order should match order of priority of treatment.  They are used in Treatment Plan and Chart "
							+"modules. They can be simple numbers or descriptive abbreviations 7 letters or less.  Changes affect all procedures where the "
							+"definition is used.  'Internal Priority' does not show, but is used for list order and for automated selection of which procedures "
							+"are next in a planned appointment.");
						break;
					case DefCat.WebSchedExistingApptTypes:
						defCOption.CanDelete=true;
						defCOption.CanHide=false;
						defCOption.ValueText="Appointment Type";
						defCOption.HelpText="Appointment types to be displayed in the Web Sched Existing Patient web application.  These are selectable by patients and will be saved to the appointment note.";
						break;
					case DefCat.WebSchedNewPatApptTypes:
						defCOption.CanDelete=true;
						defCOption.CanHide=false;
						defCOption.ValueText=Lans.g("FormDefinitions","Appointment Type");
						defCOption.HelpText=Lans.g("FormDefinitions","Appointment types to be displayed in the Web Sched New Pat Appt web application.  These are selectable for the new patients and will be saved to the appointment note.");
						break;
					case DefCat.CarrierGroupNames:
						defCOption.CanHide=true;
						defCOption.HelpText=Lans.g("FormDefinitions","These are group names for Carriers.");
						break;
					case DefCat.TimeCardAdjTypes:
						defCOption.CanEditName=true;
						defCOption.CanHide=true;
						defCOption.HelpText=Lans.g("FormDefinitions","These are PTO Adjustments Types used for tracking on employee time cards and ADP export.");
						break;
				}
				listDefCatOptions.Add(defCOption);
			}
			return listDefCatOptions;
		}

		private static string GetItemDescForImages(string itemValue) {
			List<string> listVals=new List<string>();
			if(itemValue.Contains("X")) {
				listVals.Add(Lan.g(_lanThis,"ChartModule"));
			}
			if(itemValue.Contains("F")) {
				listVals.Add(Lan.g(_lanThis,"PatientForm"));
			}
			if(itemValue.Contains("P")){
				listVals.Add(Lan.g(_lanThis,"PatientPic"));
			}
			if(itemValue.Contains("S")){
				listVals.Add(Lan.g(_lanThis,"Statement"));
			}
			if(itemValue.Contains("T")){
				listVals.Add(Lan.g(_lanThis,"ToothChart"));
			}
			if(itemValue.Contains("R")) {
				listVals.Add(Lan.g(_lanThis,"TreatPlans"));
			}
			if(itemValue.Contains("L")) {
				listVals.Add(Lan.g(_lanThis,"PatientPortal"));
			}
			if(itemValue.Contains("A")) {
				listVals.Add(Lan.g(_lanThis,"PayPlans"));
			}
			if(itemValue.Contains("C")) {
				listVals.Add(Lan.g(_lanThis,"ClaimAttachments"));
			}
			if(itemValue.Contains("B")) {
				listVals.Add(Lan.g(_lanThis,"LabCases"));
			}
			if(itemValue.Contains("U")) {
				listVals.Add(Lan.g(_lanThis,"AutoSaveForms"));
			}
			return string.Join(", ",listVals);
		}
		#endregion
		///<summary>Fills the passed in grid with the definitions in the passed in list.</summary>
		public static void FillGridDefs(GridOD gridDefs,DefCatOptions selectedDefCatOpt,List<Def> listDefsCur) {
			Def selectedDef=null;
			if(gridDefs.GetSelectedIndex() > -1) {
				selectedDef=(Def)gridDefs.ListGridRows[gridDefs.GetSelectedIndex()].Tag;
			}
			int scroll=gridDefs.ScrollValue;
			gridDefs.BeginUpdate();
			gridDefs.ListGridColumns.Clear();
			GridColumn col;
			col = new GridColumn(Lan.g("TableDefs","Name"),190);
			gridDefs.ListGridColumns.Add(col);
			col = new GridColumn(selectedDefCatOpt.ValueText,190);
			gridDefs.ListGridColumns.Add(col);
			col = new GridColumn(selectedDefCatOpt.EnableColor ? Lan.g("TableDefs","Color") : "",40);
			gridDefs.ListGridColumns.Add(col);
			col = new GridColumn(selectedDefCatOpt.CanHide ? Lan.g("TableDefs","Hide") : "",30,HorizontalAlignment.Center);
			gridDefs.ListGridColumns.Add(col);
			gridDefs.ListGridRows.Clear();
			GridRow row;
			foreach(Def defCur in listDefsCur) {
				if(!PrefC.IsODHQ && defCur.ItemValue==CommItemTypeAuto.ODHQ.ToString()) {
					continue;
				}
				if(Defs.IsDefDeprecated(defCur)) {
					defCur.IsHidden=true;
				}
				row=new GridRow();
				if(selectedDefCatOpt.CanEditName) {
					row.Cells.Add(defCur.ItemName);
				}
				else {//Users cannot edit the item name so let them translate them.
					row.Cells.Add(Lan.g("FormDefinitions",defCur.ItemName));//Doesn't use 'this' so that renaming the form doesn't change the translation
				}
				if(selectedDefCatOpt.DefCat==DefCat.ImageCats) {
					row.Cells.Add(GetItemDescForImages(defCur.ItemValue));
				}
				else if(selectedDefCatOpt.DefCat==DefCat.AutoNoteCats) {
					Dictionary<string,string> dictAutoNoteDefs = new Dictionary<string,string>();
					dictAutoNoteDefs=listDefsCur.ToDictionary(x => x.DefNum.ToString(),x => x.ItemName);
					string nameCur;
					row.Cells.Add(dictAutoNoteDefs.TryGetValue(defCur.ItemValue,out nameCur) ? nameCur : defCur.ItemValue);
				}
				else if(selectedDefCatOpt.DefCat==DefCat.WebSchedNewPatApptTypes || selectedDefCatOpt.DefCat==DefCat.WebSchedExistingApptTypes) {
					AppointmentType appointmentType=AppointmentTypes.GetApptTypeForDef(defCur.DefNum);
					row.Cells.Add(appointmentType==null ? "" : appointmentType.AppointmentTypeName);
				}
				else if(selectedDefCatOpt.DoShowItemOrderInValue) {
					row.Cells.Add(defCur.ItemOrder.ToString());
				}
				else {
					row.Cells.Add(defCur.ItemValue);
				}
				row.Cells.Add("");
				if(selectedDefCatOpt.EnableColor) {
					row.Cells[row.Cells.Count-1].ColorBackG=defCur.ItemColor;
				}
				if(defCur.IsHidden) {
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}
				row.Tag=defCur;
				gridDefs.ListGridRows.Add(row);
			}
			gridDefs.EndUpdate();
			if(selectedDef!=null) {
				for(int i=0;i < gridDefs.ListGridRows.Count;i++) {
					if(((Def)gridDefs.ListGridRows[i].Tag).DefNum == selectedDef.DefNum) {
						gridDefs.SetSelected(i,true);
						break;
					}
				}
			}
			gridDefs.ScrollValue=scroll;
		}

		public static bool GridDefsDoubleClick(Def selectedDef,GridOD gridDefs,DefCatOptions selectedDefCatOpt,List<Def> listDefsCur,List<Def> listDefsAll,bool isDefChanged) {
			switch(selectedDefCatOpt.DefCat) {
				case DefCat.BlockoutTypes:
					using(FormDefEditBlockout FormDEB=new FormDefEditBlockout(selectedDef)) {
						FormDEB.ShowDialog();
						if(FormDEB.DialogResult==DialogResult.OK) {
							isDefChanged=true;
						}
					}
					break;
				case DefCat.ImageCats:
					using(FormDefEditImages FormDEI=new FormDefEditImages(selectedDef)) {
						FormDEI.IsNew=false;
						FormDEI.ShowDialog();
						if(FormDEI.DialogResult==DialogResult.OK) {
							isDefChanged=true;
						}
					}
					break;
				case DefCat.WebSchedExistingApptTypes:
					using(FormDefEditWebSchedApptTypes FormDEWSEP=new FormDefEditWebSchedApptTypes(selectedDef,"Edit Web Sched Existing Patient Appt Type")) {
						if(FormDEWSEP.ShowDialog()==DialogResult.OK) {
							if(FormDEWSEP.IsDeleted) {
								listDefsAll.Remove(selectedDef);
							}
							isDefChanged=true;
						}
					}
					break;
				case DefCat.WebSchedNewPatApptTypes:
					using(FormDefEditWebSchedApptTypes FormDEWSNPAT=new FormDefEditWebSchedApptTypes(selectedDef,"Edit Web Sched New Patient Appt Type")) {
						if(FormDEWSNPAT.ShowDialog()==DialogResult.OK) {
							if(FormDEWSNPAT.IsDeleted) {
								listDefsAll.Remove(selectedDef);
							}
							isDefChanged=true;
						}
					}
					break;
				default://Show the normal FormDefEdit window.
					using(FormDefEdit FormDefEdit2=new FormDefEdit(selectedDef,listDefsCur,selectedDefCatOpt)) {
						FormDefEdit2.IsNew=false;
						FormDefEdit2.ShowDialog();
						if(FormDefEdit2.DialogResult==DialogResult.OK) {
							if(FormDefEdit2.IsDeleted) {
								listDefsAll.Remove(selectedDef);
							}
							isDefChanged=true;
						}
					}
					break;
			}
			return isDefChanged;
		}

		public static bool AddDef(GridOD gridDefs,DefCatOptions selectedDefCatOpt) {
			Def defCur=new Def();
			defCur.IsNew=true;
			int itemOrder=0;
			if(Defs.GetDefsForCategory(selectedDefCatOpt.DefCat).Count>0) {
				itemOrder=Defs.GetDefsForCategory(selectedDefCatOpt.DefCat).Max(x => x.ItemOrder) + 1;
			}
			defCur.ItemOrder=itemOrder;
			defCur.Category=selectedDefCatOpt.DefCat;
			defCur.ItemName="";
			defCur.ItemValue="";//necessary
			if(selectedDefCatOpt.DefCat==DefCat.InsurancePaymentType) {
				defCur.ItemValue="N";
			}
			switch(selectedDefCatOpt.DefCat) {
				case DefCat.BlockoutTypes:
					using(FormDefEditBlockout FormDEB=new FormDefEditBlockout(defCur)) {
						FormDEB.IsNew=true;
						if(FormDEB.ShowDialog()!=DialogResult.OK) {
							return false;
						}
					}
					break;
				case DefCat.ImageCats:
					using(FormDefEditImages FormDEI=new FormDefEditImages(defCur)) {
						FormDEI.IsNew=true;
						FormDEI.ShowDialog();
						if(FormDEI.DialogResult!=DialogResult.OK) {
							return false;
						}
					}
					break;
				case DefCat.WebSchedExistingApptTypes:
					using(FormDefEditWebSchedApptTypes FormDEWSApptType=new FormDefEditWebSchedApptTypes(defCur,"Edit Web Sched Existing Patient Appt Type")) {
						if(FormDEWSApptType.ShowDialog()!=DialogResult.OK) {
							return false;
						}
					}
					break;
				case DefCat.WebSchedNewPatApptTypes:
					using(FormDefEditWebSchedApptTypes FormDEWSNPAT=new FormDefEditWebSchedApptTypes(defCur,"Edit Web Sched New Patient Appt Type")) {
						if(FormDEWSNPAT.ShowDialog()!=DialogResult.OK) { 
							return false;
						}
					}
					break;
				default:
					List<Def> listCurrentDefs=new List<Def>();
					foreach(GridRow rowCur in gridDefs.ListGridRows) {
						listCurrentDefs.Add((Def)rowCur.Tag);
					}
					using(FormDefEdit FormDE=new FormDefEdit(defCur,listCurrentDefs,selectedDefCatOpt)) {
						FormDE.IsNew=true;
						FormDE.ShowDialog();
						if(FormDE.DialogResult!=DialogResult.OK) {
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
			Defs.HideDef(defSelected);
			return true;
		}

		///<summary>Returns true if definition can be hidden or is already hidden. Displays error message and returns false if not.</summary>
		public static bool CanHideDef(Def def,DefCatOptions defCatOpt) {
			if(def.IsHidden) {//Return true if Def is already hidden.
				return true;
			}
			if(!defCatOpt.CanHide || !defCatOpt.CanEditName) {
				MsgBox.Show(_lanThis,"Definitions of this category cannot be hidden.");
				return false;//We should never get here, but if we do, something went wrong because the definition shouldn't have been hideable
			}
			//Stop users from hiding the last definition in categories that must have at least one def in them.
			List<Def> listDefsCurNotHidden=Defs.GetDefsForCategory(defCatOpt.DefCat,true);
			if(Defs.NeedOneUnhidden(def.Category) && listDefsCurNotHidden.Count==1) {
				MsgBox.Show(_lanThis,"You cannot hide the last definition in this category.");
				return false;
			}
			if(def.Category==DefCat.ProviderSpecialties
				&& (Providers.IsSpecialtyInUse(def.DefNum)
				|| Referrals.IsSpecialtyInUse(def.DefNum))) 
			{
				MsgBox.Show(_lanThis,"You cannot hide a specialty if it is in use by a provider or a referral source.");
				return false;
			}
			if(Defs.IsDefinitionInUse(def)) {//DefNum will be zero if it is being created but hasn't been saved to DB yet, thus it can't be in use.
				bool isClinicDefaultBillingType=ClinicPrefs.GetPrefAllClinics(PrefName.PracticeDefaultBillType).Any(x => x.ValueString==def.DefNum.ToString());
				if(ListTools.In(def.DefNum,
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
				else if(ListTools.In(def.DefNum,
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
				else {
					if(!MsgBox.Show(_lanThis,MsgBoxButtons.OKCancel,"Warning: This definition is currently in use within the program.")) {
						return false;
					}
				}
			}
			if(def.Category==DefCat.PaySplitUnearnedType) {
				if(listDefsCurNotHidden.FindAll(x => string.IsNullOrEmpty(x.ItemValue)).Count==1 && def.ItemValue=="") {
					MsgBox.Show(_lanThis,"Must have at least one definition that shows in Account");
					return false;
				}
			}
			//Warn the user if they are about to hide a billing type currently in use.
			if(defCatOpt.DefCat==DefCat.BillingTypes && Patients.IsBillingTypeInUse(def.DefNum)) {
				if(!MsgBox.Show(_lanThis,MsgBoxButtons.OKCancel,
					"Warning: Billing type is currently in use by patients, insurance plans, or preferences.")) 
				{
					return false;
				}
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
			Defs.Update(defSelected);
			Defs.Update(defAbove);
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
			Defs.Update(defSelected);
			Defs.Update(defBelow);
			return true;
		}

	}
}
