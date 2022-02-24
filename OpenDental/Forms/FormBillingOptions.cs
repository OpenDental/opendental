using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	///<summary></summary>
	public partial class FormBillingOptions : FormODBase {
		//private FormQuery FormQuery2;
		private List<Dunning> _listDunnings;
		public long ClinicNum;
		///<summary>Key: ClinicNum, Value: List of ClinicPrefs for clinic.
		///List contains all existing ClinicPrefs.</summary>
		private Dictionary<long,List<ClinicPref>> _dictClinicPrefsOld;
		///<summary>Key: ClinicNum, Value: List of ClinicPrefs for clinic.
		///Starts off as a copy of _ListClinicPrefsOld, then is modified.</summary>
		private Dictionary<long,List<ClinicPref>> _dictClinicPrefsNew;
		///<summary>When creating list for all clinics, this stores text we show after completed.</summary>
		private string _popUpMessage;
		private List<Def> _listBillingTypeDefs;
		///<summary>Tracks if textDateStart is blank so we can display a warning in FormBilling.cs when sending electronic bills.</summary>
		public bool IsHistoryStartMinDate { get; private set; }

		///<summary></summary>
		public FormBillingOptions(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormBillingOptions_Load(object sender, System.EventArgs e) {
			textLastStatement.Text=DateTime.Today.AddMonths(-1).ToShortDateString();
			checkUseClinicDefaults.Visible=false;
			if(PrefC.HasClinicsEnabled) {
				RefreshClinicPrefs();
				labelSaveDefaults.Text="("+Lan.g(this,"except the date at the top and clinic at the bottom")+")";
				if(comboClinic.IsUnassignedSelected){
					comboClinic.IsAllSelected=true;
				}
			}
			_listBillingTypeDefs=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			listBillType.Items.Add(Lan.g(this,"(all)"));
			listBillType.Items.AddList(_listBillingTypeDefs.Select(x => x.ItemName).ToArray(), x => x);
			comboAge.Items.Add(Lan.g(this,"Any Balance"));
			comboAge.Items.Add(Lan.g(this,"Over 30 Days"));
			comboAge.Items.Add(Lan.g(this,"Over 60 Days"));
			comboAge.Items.Add(Lan.g(this,"Over 90 Days"));
			listModeToText.Items.Clear();
			if(SmsPhones.IsIntegratedTextingEnabled()) {
				foreach(StatementMode stateMode in Enum.GetValues(typeof(StatementMode))) {
					listModeToText.Items.Add(Lan.g("enumStatementMode",stateMode.GetDescription()),stateMode);
				}
			}
			else {
				listModeToText.Visible=false;
				labelModesToText.Visible=false;
			}
			SetClinicFilters();
			if(!PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
				checkSuperFam.Visible=false;
			}
			//blank is allowed
			FillDunning();
			SetDefaults();
		}

		///<summary>Call when you want to populate/update _dicClinicPrefsOld and _dicClinicPrefsNew.</summary>
		private void RefreshClinicPrefs() {
			List<PrefName> listBillingPrefs=new List<PrefName>() {
				PrefName.BillingIncludeChanged,
				PrefName.BillingSelectBillingTypes,
				PrefName.BillingAgeOfAccount,
				PrefName.BillingExcludeBadAddresses,
				PrefName.BillingExcludeInactive,
				PrefName.BillingExcludeNegative,
				PrefName.BillingExcludeInsPending,
				PrefName.BillingExcludeIfUnsentProcs,
				PrefName.BillingExcludeLessThan,
				PrefName.BillingIgnoreInPerson,
				PrefName.BillingShowTransSinceBalZero,
				PrefName.BillingDefaultsNote
			};
			_dictClinicPrefsOld=ClinicPrefs.GetWhere(x => listBillingPrefs.Contains(x.PrefName))
				.GroupBy(x => x.ClinicNum)
				.ToDictionary(x => x.Key,x => x.ToList());
			_dictClinicPrefsNew=_dictClinicPrefsOld.ToDictionary(x => x.Key,x => x.Value.Select(y => y.Clone()).ToList());
			//Originally all ClinicPrefs were inserted together, so you either had none or all.
			//We have since added new ClincPrefs to this list so we need to identify missing clinicPrefs
			//Missing ClincPrefs will default to the standard preference table value.
			foreach(long clincNum in _dictClinicPrefsNew.Keys) {
				List<PrefName> listExistingClinicPrefs=_dictClinicPrefsNew[clincNum].Select(x => x.PrefName).ToList();
				List<PrefName> listMissingClincPrefs=listBillingPrefs.FindAll(x => !listExistingClinicPrefs.Contains(x));
				foreach(PrefName prefName in listMissingClincPrefs) {
					switch(EnumTools.GetAttributeOrDefault<PrefNameAttribute>(prefName).ValueType) {
						case PrefValueType.BOOL:
							bool defaultBool=PrefC.GetBool(prefName);
							_dictClinicPrefsNew[clincNum].Add(new ClinicPref(clincNum,prefName,defaultBool));
						break;
						case PrefValueType.ENUM:
							//Currently not used.
						break;
						case PrefValueType.STRING:
							string defaultStr=PrefC.GetString(prefName);
							_dictClinicPrefsNew[clincNum].Add(new ClinicPref(clincNum,prefName,defaultStr));
						break;
					}
				}
			}
		}
		
		///<summary>Called when we need to update the filter options.
		///If All, the unassigned clinic, or more than one clinic is selected, or _dicClinicPrefsNew does not 
		///contain a key for the current selected clinic, the standard preference based defaults will load.</summary>
		private void SetFiltersForClinicNums(List<long> listClinicNums,bool isTextNoteExcluded=false) {
			FillDunning();
			if(listClinicNums.Count != 1 || listClinicNums.Contains(-1) || listClinicNums.Contains(0) 
				|| !_dictClinicPrefsNew.ContainsKey(listClinicNums[0]))//They have not saved their default filter options for the selected clinic. Use default prefs.
			{
				checkIncludeChanged.Checked=PrefC.GetBool(PrefName.BillingIncludeChanged);
				#region BillTypes
				listBillType.ClearSelected();
				string[] selectedBillTypes=PrefC.GetString(PrefName.BillingSelectBillingTypes).Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries);
				foreach(string billTypeDefNum in selectedBillTypes) {
					try{
						int order=Defs.GetOrder(DefCat.BillingTypes,Convert.ToInt64(billTypeDefNum));
						if(order==-1) {
							continue;
						}
						listBillType.SetSelected(order+1,true);
					}
					catch(Exception) {//cannot convert string to int, just continue
						continue;
					}
				}
				if(listBillType.SelectedIndices.Count==0){
					listBillType.SelectedIndex=0;
				}
				#endregion
				#region Age
				switch(PrefC.GetString(PrefName.BillingAgeOfAccount)){
					default:
						comboAge.SelectedIndex=0;
						break;
					case "30":
						comboAge.SelectedIndex=1;
						break;
					case "60":
						comboAge.SelectedIndex=2;
						break;
					case "90":
						comboAge.SelectedIndex=3;
						break;
				}
				#endregion
				checkBadAddress.Checked=PrefC.GetBool(PrefName.BillingExcludeBadAddresses);
				checkExcludeInactive.Checked=PrefC.GetBool(PrefName.BillingExcludeInactive);
				checkShowNegative.Checked=!PrefC.GetBool(PrefName.BillingExcludeNegative);
				checkExcludeInsPending.Checked=PrefC.GetBool(PrefName.BillingExcludeInsPending);
				checkExcludeIfProcs.Checked=PrefC.GetBool(PrefName.BillingExcludeIfUnsentProcs);
				textExcludeLessThan.Text=PrefC.GetString(PrefName.BillingExcludeLessThan);
				checkIgnoreInPerson.Checked=PrefC.GetBool(PrefName.BillingIgnoreInPerson);
				checkBoxBillShowTransSinceZero.Checked=PrefC.GetBool(PrefName.BillingShowTransSinceBalZero);
				if(!isTextNoteExcluded) {
					textNote.Text=PrefC.GetString(PrefName.BillingDefaultsNote);
				}
				return;
			}
			else {//Update filter UI to reflect ClinicPrefs. //there has to be ONE item in ClinicNums. It MUST be in the dictionary and it MUST NOT be -1 or 0.
				List<ClinicPref> listClinicPrefs=_dictClinicPrefsNew[listClinicNums[0]];//By definition of how ClinicPrefs are created, First will always return a result.
				checkIncludeChanged.Checked=PIn.Bool(listClinicPrefs.First(x => x.PrefName==PrefName.BillingIncludeChanged).ValueString);
				#region BillTypes
				listBillType.ClearSelected();
				string[] selectedBillTypes=listClinicPrefs.First(x => x.PrefName==PrefName.BillingSelectBillingTypes).ValueString.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries);
				foreach(string billTypeDefNum in selectedBillTypes) {
					try {
						int order=Defs.GetOrder(DefCat.BillingTypes,Convert.ToInt64(billTypeDefNum));
						if(order==-1) {
							continue;
						}
						listBillType.SetSelected(order+1,true);
					}
					catch(Exception) {//cannot convert string to int, just continue
						continue;
					}
				}
				if(listBillType.SelectedIndices.Count==0) {
					listBillType.SelectedIndex=0;
				}
				#endregion
				#region Age
				switch(listClinicPrefs.First(x => x.PrefName==PrefName.BillingAgeOfAccount).ValueString) {
					default:
						comboAge.SelectedIndex=0;
						break;
					case "30":
						comboAge.SelectedIndex=1;
						break;
					case "60":
						comboAge.SelectedIndex=2;
						break;
					case "90":
						comboAge.SelectedIndex=3;
						break;
				}
				#endregion
				checkBadAddress.Checked=PIn.Bool(listClinicPrefs.First(x => x.PrefName==PrefName.BillingExcludeBadAddresses).ValueString);
				checkExcludeInactive.Checked=PIn.Bool(listClinicPrefs.First(x => x.PrefName==PrefName.BillingExcludeInactive).ValueString);
				checkShowNegative.Checked=!PIn.Bool(listClinicPrefs.First(x => x.PrefName==PrefName.BillingExcludeNegative).ValueString);
				checkExcludeInsPending.Checked=PIn.Bool(listClinicPrefs.First(x => x.PrefName==PrefName.BillingExcludeInsPending).ValueString);
				checkExcludeIfProcs.Checked=PIn.Bool(listClinicPrefs.First(x => x.PrefName==PrefName.BillingExcludeIfUnsentProcs).ValueString);
				textExcludeLessThan.Text=listClinicPrefs.First(x => x.PrefName==PrefName.BillingExcludeLessThan).ValueString;
				checkIgnoreInPerson.Checked=PIn.Bool(listClinicPrefs.First(x => x.PrefName==PrefName.BillingIgnoreInPerson).ValueString);
				checkBoxBillShowTransSinceZero.Checked=PIn.Bool(listClinicPrefs.First(x => x.PrefName==PrefName.BillingShowTransSinceBalZero).ValueString);
				if(!isTextNoteExcluded) {
					textNote.Text=listClinicPrefs.First(x => x.PrefName==PrefName.BillingDefaultsNote).ValueString;
				}
			}
		}

		private void butSaveDefault_Click(object sender,System.EventArgs e) {
			if(!textExcludeLessThan.IsValid() || !textLastStatement.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(listBillType.SelectedIndices.Count==0) {
				MsgBox.Show(this,"Please select at least one billing type first.");
				return;
			}
			string selectedBillingTypes="";//indicates all.
			if(listBillType.SelectedIndices.Count>0 && !listBillType.SelectedIndices.Contains(0)) {
				selectedBillingTypes=string.Join(",",listBillType.SelectedIndices.OfType<int>().Select(x => _listBillingTypeDefs[x-1].DefNum));
			}
			string ageOfAccount="";
			if(ListTools.In(comboAge.SelectedIndex,1,2,3)) {
				ageOfAccount=(30*comboAge.SelectedIndex).ToString();//ageOfAccount is 30, 60, or 90
			}
			List<long> listClinicNums = comboClinic.ListSelectedClinicNums;
			if(listClinicNums.Count != 1 || listClinicNums.Contains(0))//Clinics not enabled (count 0) or multiple selected(>1) or 'Unassigned' selected.
			{
				if(Prefs.UpdateBool(PrefName.BillingIncludeChanged,checkIncludeChanged.Checked)
					|Prefs.UpdateString(PrefName.BillingSelectBillingTypes,selectedBillingTypes)
					|Prefs.UpdateString(PrefName.BillingAgeOfAccount,ageOfAccount)
					|Prefs.UpdateBool(PrefName.BillingExcludeBadAddresses,checkBadAddress.Checked)
					|Prefs.UpdateBool(PrefName.BillingExcludeInactive,checkExcludeInactive.Checked)
					|Prefs.UpdateBool(PrefName.BillingExcludeNegative,!checkShowNegative.Checked)
					|Prefs.UpdateBool(PrefName.BillingExcludeInsPending,checkExcludeInsPending.Checked)
					|Prefs.UpdateBool(PrefName.BillingExcludeIfUnsentProcs,checkExcludeIfProcs.Checked)
					|Prefs.UpdateString(PrefName.BillingExcludeLessThan,textExcludeLessThan.Text)
					|Prefs.UpdateBool(PrefName.BillingIgnoreInPerson,checkIgnoreInPerson.Checked)
					|Prefs.UpdateString(PrefName.BillingDefaultsNote,textNote.Text))
				{
					DataValid.SetInvalid(InvalidType.Prefs);
				}
				return;
			}
			else if(_dictClinicPrefsNew.Keys.Contains(listClinicNums[0])) {//ClincPrefs exist, update them.
				_dictClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingIncludeChanged).ValueString=POut.Bool(checkIncludeChanged.Checked);
				_dictClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingSelectBillingTypes).ValueString=selectedBillingTypes;
				_dictClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingAgeOfAccount).ValueString=ageOfAccount;
				_dictClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingExcludeBadAddresses).ValueString=POut.Bool(checkBadAddress.Checked);
				_dictClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingExcludeInactive).ValueString=POut.Bool(checkExcludeInactive.Checked);
				_dictClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingExcludeNegative).ValueString=POut.Bool(!checkShowNegative.Checked);
				_dictClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingExcludeInsPending).ValueString=POut.Bool(checkExcludeInsPending.Checked);
				_dictClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingExcludeIfUnsentProcs).ValueString=POut.Bool(checkExcludeIfProcs.Checked);
				_dictClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingExcludeLessThan).ValueString=textExcludeLessThan.Text;
				_dictClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingIgnoreInPerson).ValueString=POut.Bool(checkIgnoreInPerson.Checked);
				_dictClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingShowTransSinceBalZero).ValueString=POut.Bool(checkBoxBillShowTransSinceZero.Checked);
				_dictClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingDefaultsNote).ValueString=textNote.Text;
			}
			else {//No existing ClinicPrefs for the currently selected clinic in comboClinics.
				_dictClinicPrefsNew.Add(listClinicNums[0],new List<ClinicPref>());
				//Inserts new ClinicPrefs for each of the Filter options for the currently selected clinic.
				_dictClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingIncludeChanged,checkIncludeChanged.Checked));
				_dictClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingSelectBillingTypes,selectedBillingTypes));
				_dictClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingAgeOfAccount,ageOfAccount));
				_dictClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingExcludeBadAddresses,checkBadAddress.Checked));
				_dictClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingExcludeInactive,checkExcludeInactive.Checked));
				_dictClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingExcludeNegative,!checkShowNegative.Checked));
				_dictClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingExcludeInsPending,checkExcludeInsPending.Checked));
				_dictClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingExcludeIfUnsentProcs,checkExcludeIfProcs.Checked));
				_dictClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingExcludeLessThan,textExcludeLessThan.Text));
				_dictClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingIgnoreInPerson,checkIgnoreInPerson.Checked));
				_dictClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingShowTransSinceBalZero,checkBoxBillShowTransSinceZero.Checked));
				_dictClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingDefaultsNote,textNote.Text));
			}
			if(ClinicPrefs.Sync(_dictClinicPrefsNew.SelectMany(x => x.Value).ToList(),_dictClinicPrefsOld.SelectMany(x => x.Value).ToList())) {
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
				RefreshClinicPrefs();
			}
		}

		private void SetClinicFilters() {
			List<long> listSelectedClinicNums=comboClinic.ListSelectedClinicNums;
			if(listSelectedClinicNums.Count > 1) {
				butSaveDefault.Enabled=false;
			}
			else {
				butSaveDefault.Enabled=true;
			}
			checkUseClinicDefaults.Visible=listSelectedClinicNums.Count > 1;//Only visible when multiple clinics are selected 
			UpdateGenMsgUI(listSelectedClinicNums);
			SetFiltersForClinicNums(listSelectedClinicNums);//When textNote is visible then we are allowing overrides.
		}

		private void UpdateGenMsgUI(List<long> listSelectedClinicNums) {
			labelMultiClinicGenMsg.Visible=(checkUseClinicDefaults.Checked && listSelectedClinicNums.Count>1);
			textNote.Visible=(!labelMultiClinicGenMsg.Visible);//Hide if labelMultiClinicGenMsg is visible, allows text to still be set.
		}

		private void ComboClinic_SelectionChangeCommitted(object sender, EventArgs e){
			SetClinicFilters();
		}
		
		private void checkUseClinicDefaults_CheckedChanged(object sender,EventArgs e) {
			UpdateGenMsgUI(comboClinic.ListSelectedClinicNums);
		}

		private void FillDunning(){
			List<long> listClinicNums=comboClinic.ListSelectedClinicNums;
			if(comboClinic.IsAllSelected) {
				listClinicNums=new List<long>();//Empty list to allow query to run for all clinics.
			}
			else {
				//Running for specific clinic(s), can include unassigned (0).
			}
			_listDunnings=Dunnings.Refresh(listClinicNums);
			gridDun.BeginUpdate();
			gridDun.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Billing Type",80);
			gridDun.ListGridColumns.Add(col);
			col=new GridColumn("Aging",70);
			gridDun.ListGridColumns.Add(col);
			col=new GridColumn("Ins",40);
			gridDun.ListGridColumns.Add(col);
			col=new GridColumn("Message",150);
			gridDun.ListGridColumns.Add(col);
			col=new GridColumn("Bold Message",150);
			gridDun.ListGridColumns.Add(col);
			col=new GridColumn("Email",30, HorizontalAlignment.Center);
			gridDun.ListGridColumns.Add(col);
			gridDun.ListGridRows.Clear();
			GridRow row;
			foreach(Dunning dunnCur in _listDunnings) {
				row=new GridRow();
				if(dunnCur.BillingType==0){
					row.Cells.Add(Lan.g(this,"all"));
				}
				else{
					row.Cells.Add(Defs.GetName(DefCat.BillingTypes,dunnCur.BillingType));
				}
				if(dunnCur.AgeAccount==0){
					row.Cells.Add(Lan.g(this,"any"));
				}
				else{
					row.Cells.Add(Lan.g(this,"Over ")+dunnCur.AgeAccount.ToString());
				}
				if(dunnCur.InsIsPending==YN.Yes) {
					row.Cells.Add(Lan.g(this,"Y"));
				}
				else if(dunnCur.InsIsPending==YN.No) {
					row.Cells.Add(Lan.g(this,"N"));
				}
				else {//YN.Unknown
					row.Cells.Add(Lan.g(this,"any"));
				}
				row.Cells.Add(dunnCur.DunMessage);
				row.Cells.Add(new GridCell(dunnCur.MessageBold) { Bold=YN.Yes,ColorText=Color.DarkRed });
				if(dunnCur.EmailBody!="" || dunnCur.EmailSubject!="") {
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}
				gridDun.ListGridRows.Add(row);
			}
			gridDun.EndUpdate();
		}

		private void gridDun_CellDoubleClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			using FormDunningEdit formD=new FormDunningEdit(_listDunnings[e.Row]);
			formD.ShowDialog();
			FillDunning();
		}

		private void checkSuperFam_CheckedChanged(object sender,EventArgs e) {
			if(checkSuperFam.Checked) {
				checkIntermingled.Checked=false;
				checkIntermingled.Enabled=false;
				checkSinglePatient.Checked=false;
			}
			else {
				checkIntermingled.Enabled=true;
			}
		}

		private void butDunningSetup_Click(object sender, System.EventArgs e) {
			using FormDunningSetup formSetup=new FormDunningSetup();
			formSetup.ShowDialog();
			FillDunning();
		}

		private void butDefaults_Click(object sender,EventArgs e) {
			using FormBillingDefaults FormB=new FormBillingDefaults();
			FormB.ShowDialog();
			if(FormB.DialogResult==DialogResult.OK){
				SetDefaults();
			}
		}

		private void SetDefaults(){
			textDateStart.Text=DateTime.Today.AddDays(-PrefC.GetLong(PrefName.BillingDefaultsLastDays)).ToShortDateString();
			textDateEnd.Text=DateTime.Today.ToShortDateString();
			checkIntermingled.Checked=PrefC.GetBool(PrefName.BillingDefaultsIntermingle);
			checkSinglePatient.Checked=PrefC.GetBool(PrefName.BillingDefaultsSinglePatient);
			if(SmsPhones.IsIntegratedTextingEnabled()) {
				foreach(string modeIdx in PrefC.GetString(PrefName.BillingDefaultsModesToText)
					.Split(new string[] { "," },StringSplitOptions.RemoveEmptyEntries)) {
					listModeToText.SetSelected(PIn.Int(modeIdx),true);
				}
			}
		}

		private void checkIntermingled_Click(object sender,EventArgs e) {
			if(checkIntermingled.Checked) {
				checkSinglePatient.Checked=false;
			}
		}

		private void checkSinglePatient_Click(object sender,EventArgs e) {
			if(checkSinglePatient.Checked) {
				checkIntermingled.Checked=false;
				checkSuperFam.Checked=false;
			}
		}

		private void checkBoxBillShowTransSinceZero_CheckedChanged(object sender,EventArgs e) {
			if(checkBoxBillShowTransSinceZero.Checked) {
				textDateStart.Enabled=false;
				textDateEnd.Enabled=false;
			}
			else {
				textDateStart.Enabled=true;
				textDateEnd.Enabled=true;
			}
		}

		private void but30days_Click(object sender,EventArgs e) {
			SetAccountHistoryControl();
			textDateStart.Text=DateTime.Today.AddDays(-30).ToShortDateString();
			textDateEnd.Text=DateTime.Today.ToShortDateString();
		}

		private void but45days_Click(object sender,EventArgs e) {
			SetAccountHistoryControl();
			textDateStart.Text=DateTime.Today.AddDays(-45).ToShortDateString();
			textDateEnd.Text=DateTime.Today.ToShortDateString();
		}

		private void but90days_Click(object sender,EventArgs e) {
			SetAccountHistoryControl();
			textDateStart.Text=DateTime.Today.AddDays(-90).ToShortDateString();
			textDateEnd.Text=DateTime.Today.ToShortDateString();
		}

		private void butDatesAll_Click(object sender,EventArgs e) {
			SetAccountHistoryControl();
			textDateStart.Text="";
			textDateEnd.Text=DateTime.Today.ToShortDateString();
		}
		
		private void SetAccountHistoryControl() {
			checkBoxBillShowTransSinceZero.Checked=false;
		}

		private void butUndo_Click(object sender,EventArgs e) {
			MsgBox.Show(this,"When the billing list comes up, use the radio button at the top to show the 'Sent' bills.\r\nThen, change their status back to unsent.\r\nYou can edit them as a group using the button at the right.");
			DialogResult=DialogResult.OK;//will trigger ContrStaff to bring up the billing window
		}

		private bool RunAgingEnterprise() {
			DateTime dtNow=MiscData.GetNowDateTime();
			DateTime dtToday=dtNow.Date;
			DateTime dateLastAging=PrefC.GetDate(PrefName.DateLastAging);
			if(dateLastAging.Date==dtToday) {
				return true;//already ran aging for this date, just move on
			}
			Prefs.RefreshCache();
			DateTime dateTAgingBeganPref=PrefC.GetDateT(PrefName.AgingBeginDateTime);
			if(dateTAgingBeganPref>DateTime.MinValue) {
				MessageBox.Show(this,Lan.g(this,"In order to create statments, aging must be calculated, but you cannot run aging until it has finished the "
					+"current calculations which began on")+" "+dateTAgingBeganPref.ToString()+".\r\n"+Lans.g(this,"If you believe the current aging process "
					+"has finished, a user with SecurityAdmin permission can manually clear the date and time by going to Setup | Miscellaneous and pressing "
					+"the 'Clear' button."));
				return false;
			}
			SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Starting Aging - Billing Options");
			Prefs.UpdateString(PrefName.AgingBeginDateTime,POut.DateT(dtNow,false));//get lock on pref to block others
			Signalods.SetInvalid(InvalidType.Prefs);//signal a cache refresh so other computers will have the updated pref as quickly as possible
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				Ledgers.ComputeAging(0,dtToday);
				Prefs.UpdateString(PrefName.DateLastAging,POut.Date(dtToday,false));
			};
			progressOD.StartingMessage=Lan.g(this,"Calculating enterprise aging for all patients as of")+" "+dtToday.ToShortDateString()+"...";
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				Ledgers.AgingExceptionHandler(ex,this);
			}
			Prefs.UpdateString(PrefName.AgingBeginDateTime,"");//clear lock on pref whether aging was successful or not
			Signalods.SetInvalid(InvalidType.Prefs);
			if(progressOD.IsCancelled){
				return false;
			}
			SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Aging completed - Billing Options");
			return progressOD.IsSuccess;
		}

		///<summary>Returns the mode for the statement.</summary>
		private StatementMode GetStatementMode(PatAging patAge) {
			StatementMode mode;
			if(ListTools.In(PrefC.GetInt(PrefName.BillingUseElectronic),1,2,3,4)) {
				mode=StatementMode.Electronic;
			}
			else {
				mode=StatementMode.Mail;
			}
			Def billingType=Defs.GetDef(DefCat.BillingTypes,patAge.BillingType);
			if(billingType != null && billingType.ItemValue=="E") {
				mode=StatementMode.Email;
			}
			return mode;
		}

		private bool DoSendSms(PatAging patAge,Dictionary<long,PatAgingData> dictPatAgingData) {
			PatAgingData patAgingData;
			dictPatAgingData.TryGetValue(patAge.PatNum,out patAgingData);
			if(ListTools.In(GetStatementMode(patAge),listModeToText.GetListSelected<StatementMode>())
				&& patAgingData!=null
				&& patAgingData.PatComm!=null
				&& patAgingData.PatComm.IsSmsAnOption) 
			{
				return true;
			}
			return false;
		}

		private void butCreate_Click(object sender, System.EventArgs e) {
			if(!textExcludeLessThan.IsValid()
				|| !textLastStatement.IsValid()
				|| !textDateStart.IsValid()
				|| !textDateEnd.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(PrefC.GetBool(PrefName.AgingIsEnterprise)) {
				if(!RunAgingEnterprise()) {
					return;
				}
			}
			else if(!PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)) {
				SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Starting Aging - Billing Options");
				DateTime asOfDate=(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)?PrefC.GetDate(PrefName.DateLastAging):DateTime.Today);
				ProgressOD progressOD=new ProgressOD();
				progressOD.ActionMain=() => Ledgers.RunAging();
				progressOD.StartingMessage=Lan.g(this,"Calculating aging for all patients as of")+" "+asOfDate.ToShortDateString()+"...";
				try{
					progressOD.ShowDialogProgress();
				}
				catch(Exception ex){
					Ledgers.AgingExceptionHandler(ex,this,true);
					return;
				}
				if(progressOD.IsCancelled){
					return;
				}
				SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Aging completed - Billing Options");
			}
			//All places in the program that have the ability to run aging against the entire database require the Setup permission because it can take a long time.
			if(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily) 
				&& Security.IsAuthorized(Permissions.Setup,true)
				&& PrefC.GetDate(PrefName.DateLastAging) < DateTime.Today.AddDays(-15)) 
			{
				MsgBox.Show(this,"Last aging date seems old, so you will now be given a chance to update it.  The billing process will continue whether or not aging gets updated.");
				using FormAging FormA=new FormAging();
				FormA.BringToFront();
				FormA.ShowDialog();
			}
			int countCreated=0;
			if(comboClinic.ListSelectedClinicNums.Count > 1) { 
				countCreated=CreateManyHelper(comboClinic.ListSelectedClinicNums);
			}
			else if(comboClinic.ListSelectedClinicNums.Count == 1) { 
				countCreated=CreateHelper(comboClinic.ListSelectedClinicNums[0]);
			}
			else { //Clinics are not enabled.
				countCreated=CreateHelper(-2);
			}
			if(countCreated>0){
				DialogResult=DialogResult.OK;
			}
		}

		///<summary>Generates statements for all clinics passed in. Returns number of statements created, or -1 if there was an error or user cancelled.</summary>
		private int CreateManyHelper(List<long> listClinicNums) {
			Cursor=Cursors.WaitCursor;
			//Action actionClosingProgress=ODProgress.Show(ODEventType.Billing,typeof(BillingEvent),Lan.g(this,"Creating Billing Lists")+"...");
			Dictionary<long,Dictionary<long,PatAgingData>> dictionaryClinicPatAgingData=AgingData.GetAgingData(checkSinglePatient.Checked,checkIncludeChanged.Checked,
					checkExcludeInsPending.Checked,checkExcludeIfProcs.Checked,checkSuperFam.Checked,listClinicNums)
				.GroupBy(x => x.Value.ClinicNum)
				.ToDictionary(x => x.Key,x => x.ToDictionary(y => y.Key,y => y.Value));
			int countAllClinics=0;
			for(int i=0;i<listClinicNums.Count;i++) {
				//BillingEvent.Fire(ODEventType.Billing,Lan.g(this,"Creating Billing Lists")+"..."+"("+(i+1).ToString()+"/"
				//	+listClinicNums.Count.ToString()+")");
				//jordan If we really want the above line, then we would use the chained series example pattern.
				//The log string would be class wide because there is no progress bar at this exact location.
				if(!dictionaryClinicPatAgingData.TryGetValue(listClinicNums[i],out Dictionary<long,PatAgingData> dictionaryPatAgingData)) {
					dictionaryPatAgingData=new Dictionary<long,PatAgingData>();
				}
				int countThisClinic=CreateHelper(listClinicNums[i],checkUseClinicDefaults.Checked,true,dictionaryPatAgingData);
				if(countThisClinic==-1){
					Cursor=Cursors.Default;
					return -1;
				}
				countAllClinics+=countThisClinic;
			}
			//actionClosingProgress?.Invoke();
			Cursor=Cursors.Default;
			if(string.IsNullOrEmpty(_popUpMessage)) {
				return -1;//not sure how this could happen
			}
			if(countAllClinics==0){
				Cursor=Cursors.Default;
				MsgBox.Show(this,"List of created bills is empty for the specified filters.");
				return 0;
			}
			using MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(_popUpMessage);
			msgBox.Text=Lan.g(this,"Billing List Results");
			msgBox.ShowDialog();
			return countAllClinics;
		}

		///<summary>Creates statements for a single clinic.  Returns number of statements created, or -1 if there was an error or user cancelled.</summary>
		///<param name="clinicNum">The clinic that billing is being generated for. Passing in a value of -2 indicates clinics not being enabled.</param>
		///<param name="useClinicPrefs">Indicates to override what is shown in the UI and use the defaul values for a clinic.</param>
		///<param name="suppressPopup">Set to true to turn off any message boxes from appearing.</param>
		///<param name="dictPatAgingData">A dictionary of key=PatNum, value=PatAgingData, which is an object that stores aging data for the patient
		///regardless of clinics. Pass in to avoid calculating these values from within this function.</param>
		private int CreateHelper(long clinicNum,bool useClinicPrefs=false,bool suppressPopup=false,Dictionary<long,PatAgingData> dictPatAgingData=null) {
			if(useClinicPrefs) {//Clincs must be enabled.
				//If textNote is visible then one of the following must be true;
				//- Clinics are not enabled (does not apply since useClincPrefs is only true when using clinics)
				//- Clinics enabled, 'All' is NOT selected
				//- Clinics enabled, single clinic or unassigned is selected
				//If textNote is visible then we must be overriding the PrefName.BillingDefaultsNote with whatever is currently typed
				//,so do not update/change textNote in SetFiltersForClinicNums(...).
				SetFiltersForClinicNums(new List<long>() { clinicNum },textNote.Visible);
			}
			else {
				//Maintain current filter selections.
			}
			//If clinics are not enabled, the following list will be empty. Otherwise, it will be filled with the passed in clinicNum.
			List<long> listClinicNums=new List<long>();
			if(PrefC.HasClinicsEnabled && clinicNum >= 0) {
				listClinicNums.Add(clinicNum);
			}
			DateTime lastStatement=PIn.Date(textLastStatement.Text);
			if(textLastStatement.Text=="") {
				lastStatement=DateTime.Today;
			}
			string getAge="";
			if(comboAge.SelectedIndex==1) getAge="30";
			else if(comboAge.SelectedIndex==2) getAge="60";
			else if(comboAge.SelectedIndex==3) getAge="90";
			List<long> billingNums=new List<long>();//[listBillType.SelectedIndices.Count];
			for(int i=0;i<listBillType.SelectedIndices.Count;i++){
				if(listBillType.SelectedIndices[i]==0){//if (all) is selected, then ignore any other selections
					billingNums.Clear();
					break;
				}
				billingNums.Add(_listBillingTypeDefs[listBillType.SelectedIndices[i]-1].DefNum);
			}
			List<PatAging> listPatAging=new List<PatAging>();
			if(dictPatAgingData==null) {//If not passed in, we must generate the data here.
				dictPatAgingData=AgingData.GetAgingData(checkSinglePatient.Checked,checkIncludeChanged.Checked,checkExcludeInsPending.Checked,
					checkExcludeIfProcs.Checked,checkSuperFam.Checked,listClinicNums);
			}
			//This method was frequently causing a Middle Tier error. Grab all information, filtered by clinic if clinics enabled, and use the
			//PatAgingData dictionary to create a list of PatNums with unsent procs, a list of PatNums with pending ins, and a dictionary of PatNum key to
			//last trans date value and send only that data as it's all that GetAgingList needs.
			List<long> listPendingInsPatNums=new List<long>();
			List<long> listUnsentPatNums=new List<long>();
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTransactions=new SerializableDictionary<long,List<PatAgingTransaction>>();
			if(checkExcludeInsPending.Checked || checkExcludeIfProcs.Checked || checkIncludeChanged.Checked) {
				foreach(KeyValuePair<long,PatAgingData> kvp in dictPatAgingData) {
					if(PrefC.HasClinicsEnabled && !listClinicNums.IsNullOrEmpty() && !listClinicNums.Contains(kvp.Value.ClinicNum)) {
						continue;
					}
					if(checkExcludeInsPending.Checked && kvp.Value.HasPendingIns) {//don't fill list if not excluding if pending ins
						listPendingInsPatNums.Add(kvp.Key);
					}
					if(checkExcludeIfProcs.Checked && kvp.Value.HasUnsentProcs) {//don't fill list if not excluding if unsent procs
						listUnsentPatNums.Add(kvp.Key);
					}
					if(checkIncludeChanged.Checked) {//don't fill dictionary if not including if changed
						dictPatAgingTransactions[kvp.Key]=kvp.Value.ListPatAgingTransactions;
					}
				}
			}
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => listPatAging=Patients.GetAgingList(getAge,lastStatement,billingNums,checkBadAddress.Checked,!checkShowNegative.Checked,
					PIn.Double(textExcludeLessThan.Text),checkExcludeInactive.Checked,checkIgnoreInPerson.Checked,listClinicNums,checkSuperFam.Checked,
					checkSinglePatient.Checked,listPendingInsPatNums,listUnsentPatNums,dictPatAgingTransactions);
			try {
				progressOD.ShowDialogProgress();
			}
			catch (Exception ex){
				string text=Lan.g(this,"Error getting list:")+" "+ex.Message+"\r\n\n\n"+ex.StackTrace;
				if(ex.InnerException!=null) {
					text+="\r\n\r\nInner Exception: "+ex.InnerException.Message+"\r\n\r\n"+ex.InnerException.StackTrace;
				}
				Cursor=Cursors.Default;
				using MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(text);
				msgBox.ShowDialog();
				return -1;
			}
			if(progressOD.IsCancelled){
				Cursor=Cursors.Default;
				return -1;
			}
			List<Patient> listSuperHeadPats=new List<Patient>();
			//If making a super family bill, we need to manipulate the agingList to only contain super family head members.
			//It can also contain regular family members, but should not contain any individual super family members other than the head.
			if(checkSuperFam.Checked) {
				List<PatAging> listSuperAgings=new List<PatAging>();
				for(int i=listPatAging.Count-1;i>=0;i--) {//Go through each PatAging in the retrieved list
					if(listPatAging[i].SuperFamily==0 || !listPatAging[i].HasSuperBilling) {
						continue;//It is okay to leave non-super billing PatAgings in the list.
					}
					Patient superHead=listSuperHeadPats.FirstOrDefault(x => x.PatNum==listPatAging[i].SuperFamily);
					if(superHead==null) {
						superHead=Patients.GetPat(listPatAging[i].SuperFamily);
						listSuperHeadPats.Add(superHead);
					}
					if(!superHead.HasSuperBilling) {
						listPatAging[i].HasSuperBilling=false;//Family guarantor has super billing but superhead doesn't, so no super bill.  Mark statement as no superbill.
						continue;
					}
					//If the guar has super billing enabled and the superhead has superbilling, this entry needs to be merged to superbill.
					if(listPatAging[i].HasSuperBilling && superHead.HasSuperBilling) {
						PatAging patA=listSuperAgings.FirstOrDefault(x => x.PatNum==superHead.PatNum);//Attempt to find an existing PatAging for the superhead.
						if(patA==null) {
							//Create new PatAging object using SuperHead "credentials" but the guarantor's balance information.
							patA=new PatAging();
							patA.AmountDue=listPatAging[i].AmountDue;
							patA.BalTotal=listPatAging[i].BalTotal;
							patA.Bal_0_30=listPatAging[i].Bal_0_30;
							patA.Bal_31_60=listPatAging[i].Bal_31_60;
							patA.Bal_61_90=listPatAging[i].Bal_61_90;
							patA.BalOver90=listPatAging[i].BalOver90;
							patA.InsEst=listPatAging[i].InsEst;
							patA.PatName=superHead.GetNameLF();
							patA.DateLastStatement=listPatAging[i].DateLastStatement;
							patA.BillingType=superHead.BillingType;
							patA.PayPlanDue=listPatAging[i].PayPlanDue;
							patA.PatNum=superHead.PatNum;
							patA.HasSuperBilling=listPatAging[i].HasSuperBilling;//true
							patA.SuperFamily=listPatAging[i].SuperFamily;//Same as superHead.PatNum
							patA.ClinicNum=listPatAging[i].ClinicNum;
							listSuperAgings.Add(patA);
						}
						else {
							//Sum the information together for all guarantors of the superfamily.
							patA.AmountDue+=listPatAging[i].AmountDue;
							patA.BalTotal+=listPatAging[i].BalTotal;
							patA.Bal_0_30+=listPatAging[i].Bal_0_30;
							patA.Bal_31_60+=listPatAging[i].Bal_31_60;
							patA.Bal_61_90+=listPatAging[i].Bal_61_90;
							patA.BalOver90+=listPatAging[i].BalOver90;
							patA.InsEst+=listPatAging[i].InsEst;
							patA.PayPlanDue+=listPatAging[i].PayPlanDue;
						}
						listPatAging.RemoveAt(i);//Remove the individual guarantor statement from the aging list since it's been combined with a superstatement.
					}
				}
				listPatAging.AddRange(listSuperAgings);
			}
			#region Message Construction
			if(PrefC.HasClinicsEnabled) {
				string clinicAbbr;
				switch(clinicNum) {
					case -1://All
					case -2:
						clinicAbbr=Lan.g(this,"All");//clinics not enabled, which is not quite the same as All because this method is just for one clinic
						break;
					case 0://Unassigned
						clinicAbbr=Lan.g(this,"Unassigned");
						break;
					default:
						clinicAbbr=Clinics.GetAbbr(clinicNum);
						break;
				}
				_popUpMessage+=Lan.g(this,clinicAbbr)+" - ";
			}
			if(listPatAging.Count==0){
				if(!suppressPopup) {
					Cursor=Cursors.Default;
					MsgBox.Show(this,"List of created bills is empty.");
				}
				else {
					_popUpMessage+=Lan.g(this,"List of created bills is empty.")+"\r\n";
				}
				return 0;//this window will stay open
			}
			else {
				_popUpMessage+=Lan.g(this,"Statements created")+": "+POut.Int(listPatAging.Count)+"\r\n";
			}
			#endregion
			IsHistoryStartMinDate=string.IsNullOrWhiteSpace(textDateStart.Text);
			DateTime dateRangeFrom=PIn.Date(textDateStart.Text);
			DateTime dateRangeTo=DateTime.Today;//Needed for payplan accuracy.//new DateTime(2200,1,1);
			if(textDateEnd.Text!=""){
				dateRangeTo=PIn.Date(textDateEnd.Text);
			}
			Statement stmt;
			DateTime dateAsOf=DateTime.Today;//used to determine when the balance on this date began
			if(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)) {//if aging calculated monthly, use the last aging date instead of today
				dateAsOf=PrefC.GetDate(PrefName.DateLastAging);
			}
			List<WebServiceMainHQProxy.ShortGuidResult> listShortGuidUrls=new List<WebServiceMainHQProxy.ShortGuidResult>();
			SheetDef stmtSheet=SheetUtil.GetStatementSheetDef();
			if(//They are going to send texts
				listModeToText.SelectedIndices.Count > 0 
				//Or the email body has a statement URL
				|| PrefC.GetString(PrefName.BillingEmailBodyText).ToLower().Contains("[statementurl]")
				|| PrefC.GetString(PrefName.BillingEmailBodyText).ToLower().Contains("[statementshorturl]")
				//Or the statement sheet has a URL field
				|| (stmtSheet!=null && stmtSheet.SheetFieldDefs.Any(x => x.FieldType==SheetFieldType.OutputText 
							&& (x.FieldValue.ToLower().Contains("[statementurl]") || x.FieldValue.ToLower().Contains("[statementshorturl]"))))) 
			{
				//Then get some short GUIDs and URLs from OD HQ.
				try {
					int countForSms=listPatAging.Count(x => DoSendSms(x,dictPatAgingData));
					listShortGuidUrls=WebServiceMainHQProxy.GetShortGUIDs(listPatAging.Count,countForSms,clinicNum,eServiceCode.PatientPortalViewStatement);
				}
				catch(Exception ex) {
					FriendlyException.Show(Lans.g(this,"Unable to create a unique URL for each statement. The Patient Portal URL will be used instead."),ex);
					listShortGuidUrls=listPatAging.Select(x => new WebServiceMainHQProxy.ShortGuidResult {
						MediumURL=PrefC.GetString(PrefName.PatientPortalURL),
						ShortURL=PrefC.GetString(PrefName.PatientPortalURL),
						ShortGuid=""
					}).ToList();
				}
			}
			Dictionary<long,InstallmentPlan> dictInstallmentPlans=InstallmentPlans.GetForFams(listPatAging.Select(x => x.PatNum).ToList());
			List<Statement> listStatementsForInsert=new List<Statement>();
			DateTime dateBalBeganCur;
			foreach(PatAging patAgeCur in listPatAging) {
				stmt=new Statement();
				stmt.DateRangeFrom=dateRangeFrom;
				stmt.DateRangeTo=dateRangeTo;
				stmt.DateSent=DateTime.Today;
				stmt.DocNum=0;
				stmt.HidePayment=false;
				stmt.Intermingled=checkIntermingled.Checked;
				stmt.IsSent=false;
				stmt.Mode_=GetStatementMode(patAgeCur);
				if(ListTools.In(PrefC.GetInt(PrefName.BillingUseElectronic),1,2,3,4)) {
					stmt.Intermingled=true;
				}
				bool doSendSms=DoSendSms(patAgeCur,dictPatAgingData);
				stmt.SmsSendStatus=(doSendSms ? AutoCommStatus.SendNotAttempted : AutoCommStatus.DoNotSend);
				stmt.ShortGUID="";
				stmt.StatementURL="";
				stmt.StatementShortURL="";
				WebServiceMainHQProxy.ShortGuidResult shortGuidUrl=listShortGuidUrls.FirstOrDefault(x => x.IsForSms==doSendSms);
				if(shortGuidUrl!=null) {
					stmt.ShortGUID=shortGuidUrl.ShortGuid;
					stmt.StatementURL=shortGuidUrl.MediumURL;
					stmt.StatementShortURL=shortGuidUrl.ShortURL;
					listShortGuidUrls.Remove(shortGuidUrl);
				}
				stmt.Note=textNote.Text;
				InstallmentPlan installPlan;
				if(dictInstallmentPlans.TryGetValue(patAgeCur.PatNum,out installPlan) && installPlan!=null) {
					stmt.Note=textNote.Text.Replace("[InstallmentPlanTerms]","Installment Plan\r\n"
						+"Date First Payment: "+installPlan.DateFirstPayment.ToShortDateString()+"\r\n"
						+"Monthly Payment: "+installPlan.MonthlyPayment.ToString("c")+"\r\n"
						+"APR: "+(installPlan.APR/100).ToString("P")+"\r\n"
						+"Note: "+installPlan.Note);
				}
				PatAgingData patAgingData;
				dictPatAgingData.TryGetValue(patAgeCur.PatNum,out patAgingData);
				//appointment reminders are not handled here since it would be too slow.
				DateTime dateBalZeroCur=patAgingData?.DateBalZero??DateTime.MinValue;//dateBalZeroCur will be DateTime.MinValue if PatNum isn't in dict
				if(checkBoxBillShowTransSinceZero.Checked && dateBalZeroCur.Year > 1880) {
					stmt.DateRangeFrom=dateBalZeroCur;
				}
				//dateBalBegan is first transaction date for a charge that consumed the last of the credits for the account, so first transaction that isn't
				//fully paid for based on oldest paid first logic
				dateBalBeganCur=patAgingData?.DateBalBegan??DateTime.MinValue;//dateBalBeganCur will be DateTime.MinValue if PatNum isn't in dict
				int ageAccount=0;
				//ageAccount is number of days between the day the account first started to have a positive bal and the asOf date
				if(dateBalBeganCur>DateTime.MinValue) {
					ageAccount=(dateAsOf-dateBalBeganCur).Days;
				}
				List<Dunning> listDunningForPat=_listDunnings.FindAll(x => (x.BillingType==0 || x.BillingType==patAgeCur.BillingType) //same billing type
					&& x.ClinicNum==patAgeCur.ClinicNum
					&& ageAccount>=x.AgeAccount-x.DaysInAdvance //old enough to qualify for this dunning message, taking into account DaysInAdvance
					&& (x.InsIsPending==YN.Unknown || x.InsIsPending==(patAgeCur.InsEst>0?YN.Yes:YN.No)));//dunning msg ins pending=unkown or matches this acct
				Dunning dunning=listDunningForPat.LastOrDefault(x => !x.IsSuperFamily);
				if(patAgeCur.HasSuperBilling && patAgeCur.PatNum==patAgeCur.SuperFamily && checkSuperFam.Checked) {
					dunning=listDunningForPat.LastOrDefault(x => x.IsSuperFamily);
					stmt.SuperFamily=patAgeCur.SuperFamily;//If this bill is for the superhead and has superbill enabled, it's a superbill.  Flag it as such.
				}
				if(dunning!=null){
					if(stmt.Note!=""){
						stmt.Note+="\r\n\r\n";//leave one empty line
					}
					stmt.Note+=dunning.DunMessage;
					stmt.NoteBold=dunning.MessageBold;
					stmt.EmailSubject=dunning.EmailSubject;					
					stmt.EmailBody=dunning.EmailBody;
				}
				stmt.PatNum=patAgeCur.PatNum;
				stmt.SinglePatient=checkSinglePatient.Checked;
				stmt.IsBalValid=true;
				stmt.BalTotal=patAgeCur.BalTotal;
				stmt.InsEst=patAgeCur.InsEst;
				listStatementsForInsert.Add(stmt);
			}
			if(listStatementsForInsert.Count>0) {
				progressOD=new ProgressOD();
				progressOD.StartingMessage=Lan.g(this,"Inserting Statements to database")+"...";
				progressOD.ActionMain=() => Statements.InsertMany(listStatementsForInsert);
				progressOD.ShowDialogProgress();
				if(progressOD.IsCancelled){
					return -1;//don't close window
				}
			}
			return listStatementsForInsert.Count;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}




}
