/*=============================================================================================================
Open Dental GPL license Copyright (C) 2003  Jordan Sparks, DMD.  http://www.open-dent.com,  www.docsparks.com
See header in FormOpenDental.cs for complete text.  Redistributions must retain this text.
===============================================================================================================*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	///<summary></summary>
	public partial class FormDefEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		private Def _defCur;
		//private Def 
		private List<long> _listExcludeSendNums;
		private List<long> _listExcludeConfirmNums;
		///<summary>A list of DefNums that represent all of the Confirmation Statuses that should skip sending eReminders.</summary>
		private List<long> _listExcludeRemindNums;
		private List<long> _listExcludeThanksNums;
		private List<long> _listExcludeArrivalSendNums;
		private List<long> _listExcludeArrivalResponseNums;
		private List<long> _listExcludeEClipboardNums;
		private List<long> _listByodEnabled;
		private DefCatOptions _defCatOptions;
		private string _selectedValueString;
		public bool IsDeleted=false;
		///<summary>The list of definitions that is showing in FormDefinitions.  This list will typically be out of synch with the cache.  Gets set in the constructor.</summary>
		private List<Def> _listDefs;
		
		///<summary>defCur should be the currently selected def from FormDefinitions.  listDef is going to be the in-memory list of definitions currently displaying to the user.  listDef typically is out of synch with the cache which is why we need to pass it in.</summary>
		public FormDefEdit(Def defCur,List<Def> listDefs,DefCatOptions defCatOptions){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
			_defCur=defCur;
			_defCatOptions=defCatOptions;
			_listDefs=listDefs;
		}

		private void FormDefEdit_Load(object sender, System.EventArgs e) {
			if(_defCur.Category==DefCat.ApptConfirmed) {
				_listExcludeSendNums=PrefC.GetString(PrefName.ApptConfirmExcludeESend).Split(',').Select(x => PIn.Long(x)).ToList();
				_listExcludeConfirmNums=PrefC.GetString(PrefName.ApptConfirmExcludeEConfirm).Split(',').Select(x => PIn.Long(x)).ToList();
				_listExcludeRemindNums=PrefC.GetString(PrefName.ApptConfirmExcludeERemind).Split(',').Select(x => PIn.Long(x)).ToList();
				_listExcludeThanksNums=PrefC.GetString(PrefName.ApptConfirmExcludeEThankYou).Split(',').Select(x => PIn.Long(x)).ToList();
				_listExcludeArrivalSendNums=PrefC.GetString(PrefName.ApptConfirmExcludeArrivalSend).Split(',').Select(x => PIn.Long(x)).ToList();
				_listExcludeArrivalResponseNums=PrefC.GetString(PrefName.ApptConfirmExcludeArrivalResponse).Split(',').Select(x => PIn.Long(x)).ToList();
				_listExcludeEClipboardNums=PrefC.GetString(PrefName.ApptConfirmExcludeEclipboard).Split(',').ToList().Select(x => PIn.Long(x)).ToList();
				_listByodEnabled=PrefC.GetString(PrefName.ApptConfirmByodEnabled).Split(',').ToList().Select(x => PIn.Long(x)).ToList();
				//0 will get automatically added to the list when this is the first of its kind.  We never want 0 inserted.
				_listExcludeSendNums.Remove(0);
				_listExcludeConfirmNums.Remove(0);
				_listExcludeRemindNums.Remove(0);
				_listExcludeThanksNums.Remove(0);
				_listExcludeArrivalSendNums.Remove(0);
				_listExcludeArrivalSendNums.Remove(0);
				checkIncludeSend.Checked=!_listExcludeSendNums.Contains(_defCur.DefNum);
				checkIncludeConfirm.Checked=!_listExcludeConfirmNums.Contains(_defCur.DefNum);
				checkIncludeRemind.Checked=!_listExcludeRemindNums.Contains(_defCur.DefNum);
				checkIncludeThanks.Checked=!_listExcludeThanksNums.Contains(_defCur.DefNum);
				checkIncludeArrivalSend.Checked=!_listExcludeArrivalSendNums.Contains(_defCur.DefNum);
				checkIncludeArrivalResponse.Checked=!_listExcludeArrivalResponseNums.Contains(_defCur.DefNum);
				checkIncludeEClipboard.Checked=!_listExcludeEClipboardNums.Contains(_defCur.DefNum);
				checkByod.Checked=_listByodEnabled.Contains(_defCur.DefNum);
			}
			else {
				groupEConfirm.Visible=false;
				groupBoxEReminders.Visible=false;
				groupBoxEThanks.Visible=false;
				groupBoxArrivals.Visible=false;
				groupBoxEClipboard.Visible=false;
			}
			if(ListTools.In(_defCur.DefNum,PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger),PrefC.GetLong(PrefName.AppointmentTimeDismissedTrigger),
				PrefC.GetLong(PrefName.AppointmentTimeSeatedTrigger))) 
			{
				//We never want to send confirmation or reminders to an appointment when it is in a triggered confirm status.
				checkIncludeConfirm.Enabled=false;
				checkIncludeRemind.Enabled=false;
				checkIncludeSend.Enabled=false;
				checkIncludeThanks.Enabled=false;
				checkIncludeArrivalSend.Enabled=false;
				checkIncludeArrivalResponse.Enabled=false;
				checkIncludeEClipboard.Enabled=false;
				checkIncludeConfirm.Checked=false;
				checkIncludeRemind.Checked=false;
				checkIncludeSend.Checked=false;
				checkIncludeThanks.Checked=false;
				checkIncludeArrivalSend.Checked=false;
				checkIncludeArrivalResponse.Checked=false;
				checkIncludeEClipboard.Checked=false;
			}
			if(ListTools.In(_defCur.DefNum,PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger))) {
				//There always has to be one status that automatically defaults to true for BYOD, which is the time arrived trigger confirm status
				checkByod.Enabled=false;
				checkByod.Checked=true;
			}
			string itemName=_defCur.ItemName;
			_selectedValueString=_defCur.ItemValue;
			if(!_defCatOptions.CanEditName) {
				//Allow foreign users to translate definitions that they do not have access to translate.
				//Use FormDefinitions instead of 'this' because the users will have already translated the item names in that form and no need to duplicate.
				itemName=Lan.g("FormDefinitions",_defCur.ItemName);
				textName.ReadOnly=true;
				if(!_defCur.IsHidden || Defs.IsDefDeprecated(_defCur)) {
					checkHidden.Enabled=false;//prevent hiding defs that are hard-coded into OD. Prevent unhiding defs that are deprecated.
				}
			}
			labelValue.Text=_defCatOptions.ValueText;
			if(_defCur.Category==DefCat.AdjTypes && !IsNew){
				labelValue.Text="Not allowed to change type after an adjustment is created.";
				textValue.Visible=false;
			}
			if(_defCur.Category==DefCat.BillingTypes) {
				labelValue.Text="E=Email bill, C=Collection, CE=Collection Excluded";
			}
			if(_defCur.Category==DefCat.PaySplitUnearnedType) {
				labelValue.Text="X=Do Not Show in Account or on Reports";
			}
			if(_defCur.Category==DefCat.EClipboardImageCapture) {
				labelValue.Text="Name";
				labelValue.Text="Patient instructions";
			}
			if(!_defCatOptions.EnableValue){
				labelValue.Visible=false;
				textValue.Visible=false;
			}
			if(!_defCatOptions.EnableColor){
				labelColor.Visible=false;
				butColor.Visible=false;
			}
			if(!_defCatOptions.CanHide){
				checkHidden.Visible=false;
			}
			if(!_defCatOptions.CanDelete){
				butDelete.Visible=false;
			}
			if(_defCatOptions.IsValueDefNum) {
				textValue.ReadOnly=true;
				textValue.BackColor=SystemColors.Control;
				labelValue.Text=Lan.g("FormDefinitions","Use the select button to choose a definition from the list.");
				long defNumCur=PIn.Long(_defCur.ItemValue??"");
				if(defNumCur>0) {
					textValue.Text=_listDefs.FirstOrDefault(x => defNumCur==x.DefNum)?.ItemName??"";
				}
				butSelect.Visible=true;
				butClearValue.Visible=true;
			}
			else if(_defCatOptions.DoShowItemOrderInValue) {
				labelValue.Text=Lan.g(this,"Internal Priority");
				textValue.Text=_defCur.ItemOrder.ToString();
				textValue.ReadOnly=true;
				butSelect.Visible=false;
				butClearValue.Visible=false;
			}
			else {
				textValue.Text=_defCur.ItemValue;
				butSelect.Visible=false;
				butClearValue.Visible=false;
			}
			textName.Text=itemName;
			butColor.BackColor=_defCur.ItemColor;
			checkHidden.Checked=_defCur.IsHidden;
			if(_defCatOptions.DoShowNoColor) {
				checkNoColor.Visible=true;
				//If there is no color in the database currently, make the UI match this.
				checkNoColor.Checked=(_defCur.ItemColor.ToArgb()==Color.Empty.ToArgb());
			}
		}

		private void butColor_Click(object sender, System.EventArgs e) {
			colorDialog1.Color=butColor.BackColor;
			colorDialog1.ShowDialog();
			butColor.BackColor=colorDialog1.Color;
			checkNoColor.Checked=(colorDialog1.Color.ToArgb()==Color.Empty.ToArgb());
			//textColor.Text=colorDialog1.Color.Name;
		}

		private void butSelect_Click(object sender,EventArgs e) {
			long defNumParent=PIn.Long(_defCur.ItemValue);//ItemValue could be blank, in which case defNumCur will be 0
			List<Def> listDefsMatchingParent=_listDefs.FindAll(x => x.DefNum==defNumParent);
			using FormDefinitionPicker formDefinitionPicker=new FormDefinitionPicker(_defCur.Category,listDefsMatchingParent,_defCur.DefNum);
			formDefinitionPicker.IsMultiSelectionMode=false;
			formDefinitionPicker.HasShowHiddenOption=false;
			formDefinitionPicker.ShowDialog();
			if(formDefinitionPicker.DialogResult!=DialogResult.OK) {
				return;
			}
			Def defSelected=formDefinitionPicker.ListDefsSelected.DefaultIfEmpty(new Def() { ItemName="" }).First();
			_selectedValueString=defSelected.DefNum==0?"":defSelected.DefNum.ToString();//list should have exactly one def in it, but this is safe
			textValue.Text=defSelected.ItemName;
		}

		private void butClearValue_Click(object sender,EventArgs e) {
			_selectedValueString="";
			textValue.Clear();
		}

		private void checkNoColor_CheckedChanged(object sender,EventArgs e) {
			if(checkNoColor.Checked) {//Reset to empty color to tell the user the color is disabled.
				butColor.BackColor=Color.Empty;
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//This is VERY new.  Only allowed and visible for three categories so far: supply cats, claim payment types, and claim custom tracking.
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_defCur.Category==DefCat.ClaimCustomTracking && _listDefs.Count(x => x.Category==DefCat.ClaimCustomTracking)==1
				|| _defCur.Category==DefCat.InsurancePaymentType && _listDefs.Count(x => x.Category==DefCat.InsurancePaymentType)==1
				|| _defCur.Category==DefCat.SupplyCats && _listDefs.Count(x => x.Category==DefCat.SupplyCats)==1) 
			{
				MsgBox.Show(this,"Cannot delete the last definition from this category.");
				return;
			}
			bool isAutoNoteRefresh=false;
			if(_defCur.Category==DefCat.AutoNoteCats && AutoNotes.GetExists(x => x.Category==_defCur.DefNum)) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Deleting this Auto Note Category will uncategorize some auto notes.  Delete anyway?")) {
					return;
				}
				isAutoNoteRefresh=true;
			}
			try{
				Defs.Delete(_defCur);
				IsDeleted=true;
				if(isAutoNoteRefresh) {//deleting an auto note category currently in use will uncategorize those auto notes, refresh cache
					DataValid.SetInvalid(InvalidType.AutoNotes);
				}
				DialogResult=DialogResult.OK;
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(checkHidden.Checked && !IsNew) {
				if(!DefL.CanHideDef(_defCur,_defCatOptions)) {
					return;//CanHideDef() shows error message if def cannot be hidden, then we kick out here.
				}
			}
			if(textName.Text==""){
				MsgBox.Show(this,"Name required.");
				return;
			}
			switch(_defCur.Category){
				case DefCat.AccountQuickCharge:
				case DefCat.ApptProcsQuickAdd:
					string[] stringArrayProcCodes=textValue.Text.Split(',');
					List<string> listProcCodes=new List<string>();
					for(int i=0;i<stringArrayProcCodes.Length;i++) {
						ProcedureCode procedureCode=ProcedureCodes.GetProcCode(stringArrayProcCodes[i]);
						if(procedureCode.CodeNum==0) {
							//Now check to see if the trimmed version of the code does not exist either.
							procedureCode=ProcedureCodes.GetProcCode(stringArrayProcCodes[i].Trim());
							if(procedureCode.CodeNum==0) {
								MessageBox.Show(Lan.g(this,"Invalid procedure code entered")+": "+stringArrayProcCodes[i]);
								return;
							}
						}
						listProcCodes.Add(procedureCode.ProcCode);
					}
					textValue.Text=String.Join(",",listProcCodes);
					break;
				case DefCat.AdjTypes:
					if(textValue.Text!="+" && textValue.Text!="-" && textValue.Text!="dp"){
						MessageBox.Show(Lan.g(this,"Valid values are +, -, or dp."));
						return;
					}
					break;
				case DefCat.BillingTypes:
					if(!ListTools.In(textValue.Text.ToLower(),"","e","c","ce")) 
					{
						MsgBox.Show(this,"Valid values are blank, E, C, or CE.");
						return;
					}
					break;
				case DefCat.ClaimCustomTracking:
					int value=0;
					if(!Int32.TryParse(textValue.Text,out value) || value<0) {
						MsgBox.Show(this,"Days Suppressed must be a valid non-negative number.");
						return;
					}
					break;
				case DefCat.CommLogTypes:
					List<string> listCommItemTypes=Commlogs.GetCommItemTypes().Select(x => x.GetDescription(useShortVersionIfAvailable:true)).ToList();
					if(textValue.Text!="" && !listCommItemTypes.Any(x => x==textValue.Text)) {
						MessageBox.Show(Lan.g(this,"Valid values are:")+" "+string.Join(", ",listCommItemTypes));
						return;
					}
					break;
				case DefCat.DiscountTypes:
					int discountValue;
					if(textValue.Text=="") break;
					try {
						discountValue=System.Convert.ToInt32(textValue.Text);
					}
					catch {
						MessageBox.Show(Lan.g(this,"Not a valid number"));
						return;
					}
					if(discountValue < 0 || discountValue > 100) {
						MessageBox.Show(Lan.g(this,"Valid values are between 0 and 100"));
						return;
					}
					textValue.Text=discountValue.ToString();
					break;
				/*case DefCat.FeeSchedNames:
					if(textValue.Text=="C" || textValue.Text=="c") {
						textValue.Text="C";
					}
					else if(textValue.Text=="A" || textValue.Text=="a") {
						textValue.Text="A";
					}
					else textValue.Text="";
					break;*/
				case DefCat.ImageCats:
					textValue.Text=textValue.Text.ToUpper().Replace(",","");
					if(!Regex.IsMatch(textValue.Text,@"^[XPS]*$")){
						textValue.Text="";
					}
					break;
				case DefCat.InsurancePaymentType:
					if(textValue.Text!="" && textValue.Text!="N") {
						MsgBox.Show(this,"Valid values are blank or N.");
						return;
					}
					break;
				case DefCat.OperatoriesOld:
					if(textValue.Text.Length > 5){
						MessageBox.Show(Lan.g(this,"Maximum length of abbreviation is 5."));
						return;
					}
					break;
				case DefCat.PaySplitUnearnedType:
					if(!ListTools.In(textValue.Text.ToLower(),"","x")) {
						MsgBox.Show(this,"Valid values are blank or 'X'");
						return;
					}
					Pref pref=Prefs.GetPref(PrefName.TpUnearnedType.GetDescription());
					if(_defCur.DefNum.ToString()==pref.ValueString && checkHidden.Checked) {//If the current selected Def is the default, and we're setting to hidden, set the new default to the first 'shown' Def
						List<Def> listDefsForUnearnedType=_listDefs.FindAll(x => x.Category==DefCat.PaySplitUnearnedType);
						List<Def> listDefsNonHiddenForUnearnedType=listDefsForUnearnedType.FindAll(x => x.IsHidden==false && x.DefNum!=_defCur.DefNum && x.ItemValue.ToLower().Contains("x"));
						if(listDefsNonHiddenForUnearnedType.Count==0) {
							listDefsNonHiddenForUnearnedType=listDefsForUnearnedType.FindAll(x => x.IsHidden==false && x.DefNum!=_defCur.DefNum);//Get remaining nonhidden defs.
						}
						pref.ValueString=listDefsNonHiddenForUnearnedType[0].DefNum.ToString();//Set the default TpUnearnedType to the first remaining nonhidden def. Must have atleast one non-hidden. 
						Prefs.Update(pref);//update the DB entry for the TpUnearnedType
						Prefs.UpdateValueForKey(pref);//Load the DB entry for the TpUnearnedType into cache
					}
					break;
				case DefCat.RecallUnschedStatus:
					if(textValue.Text.Length > 7){
						MessageBox.Show(Lan.g(this,"Maximum length is 7."));
						return;
					}
					break;
				case DefCat.TxPriorities:
					if(textValue.Text.Length > 7){
						MessageBox.Show(Lan.g(this,"Maximum length of abbreviation is 7."));
						return;
					}
					break;
				default:
					break;
			}//end switch DefCur.Category
			_defCur.ItemName=textName.Text;
			_defCur.ItemValue=_selectedValueString;
			if(_defCatOptions.EnableValue && !_defCatOptions.IsValueDefNum) {
				_defCur.ItemValue=textValue.Text;
			}
			if(_defCatOptions.EnableColor) {
				//If checkNoColor is checked, insert empty into the database. Otherwise, use the color they picked.
				if(checkNoColor.Checked){
					_defCur.ItemColor=Color.Empty;
				}
				else{
					_defCur.ItemColor=butColor.BackColor;
				}
			}
			_defCur.IsHidden=checkHidden.Checked;
			if(IsNew){
				Defs.Insert(_defCur);
			}
			else{
				Defs.Update(_defCur);
			}
			//Must be after the upsert so that we have access to the DefNum for new Defs.
			if(_defCur.Category==DefCat.ApptConfirmed) {
				//==================== EXCLUDE SEND ====================
				UpdateAutoCommExcludes(checkIncludeSend,_listExcludeSendNums,_defCur,PrefName.ApptConfirmExcludeESend);
				//==================== EXCLUDE CONFIRM ====================
				UpdateAutoCommExcludes(checkIncludeConfirm,_listExcludeConfirmNums,_defCur,PrefName.ApptConfirmExcludeEConfirm);
				//==================== EXCLUDE REMIND ====================
				UpdateAutoCommExcludes(checkIncludeRemind,_listExcludeRemindNums,_defCur,PrefName.ApptConfirmExcludeERemind);
				//==================== EXCLUDE THANKYOU ====================
				UpdateAutoCommExcludes(checkIncludeThanks,_listExcludeThanksNums,_defCur,PrefName.ApptConfirmExcludeEThankYou);
				//==================== EXCLUDE ARRIVAL SEND ====================
				UpdateAutoCommExcludes(checkIncludeArrivalSend,_listExcludeArrivalSendNums,_defCur,PrefName.ApptConfirmExcludeArrivalSend);
				//==================== EXCLUDE ARRIVAL RESPONSE ====================
				UpdateAutoCommExcludes(checkIncludeArrivalResponse,_listExcludeArrivalResponseNums,_defCur,PrefName.ApptConfirmExcludeArrivalResponse);
				//==================== EXCLUDE ECLIPBOARD CHECKIN ======================
				UpdateAutoCommExcludes(checkIncludeEClipboard,_listExcludeEClipboardNums,_defCur,PrefName.ApptConfirmExcludeEclipboard);
				//==================== ENABLED BYOD FOR DEF ====================
				UpdateByod(checkByod,_listByodEnabled,_defCur,PrefName.ApptConfirmByodEnabled);
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			if(_defCur.Category==DefCat.MiscColors){
				if(_defCur.ItemOrder==(int)DefCatMiscColors.MainBorder){
					SetBorderColor(DefCatMiscColors.MainBorder,_defCur.ItemColor);
				}
				if(_defCur.ItemOrder==(int)DefCatMiscColors.MainBorderOutline){
					SetBorderColor(DefCatMiscColors.MainBorderOutline,_defCur.ItemColor);
				}
				if(_defCur.ItemOrder==(int)DefCatMiscColors.MainBorderText){
					SetBorderColor(DefCatMiscColors.MainBorderText,_defCur.ItemColor);
				}
			}
			DialogResult=DialogResult.OK;
		}

		private static void UpdateAutoCommExcludes(CheckBox checkBox,List<long> listExcludeNums,Def def,PrefName prefName) {
			if(checkBox.Checked) {
				listExcludeNums.RemoveAll(x => x==def.DefNum);
			}
			else {
				listExcludeNums.Add(def.DefNum);
			}
			string toString=string.Join(",",listExcludeNums.Distinct().OrderBy(x => x));
			Prefs.UpdateString(prefName,toString);
		}

		private static void UpdateByod(CheckBox check,List<long> listExcludeNums,Def def,PrefName prefName) {
			if(check.Checked) {
				listExcludeNums.Add(def.DefNum);
			}
			else {
				listExcludeNums.RemoveAll(x => x==def.DefNum);
			}
			string strExcludeNums=string.Join(",",listExcludeNums.Distinct().OrderBy(x => x));
			Prefs.UpdateString(prefName,strExcludeNums);
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
