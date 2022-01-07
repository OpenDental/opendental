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
		private Dictionary<long,List<ClinicPref>> _dictionaryClinicPrefsOld;
		///<summary>Key: ClinicNum, Value: List of ClinicPrefs for clinic.
		///Starts off as a copy of _ListClinicPrefsOld, then is modified.</summary>
		private Dictionary<long,List<ClinicPref>> _dictionaryClinicPrefsNew;
		///<summary>When creating list for all clinics, this stores text we show after completed.</summary>
		private string _popUpMessage;
		private List<Def> _listDefsBillingType;
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
			_listDefsBillingType=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			listBillType.Items.Add(Lan.g(this,"(all)"));
			listBillType.Items.AddList(_listDefsBillingType.Select(x => x.ItemName).ToArray(), x => x);
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
			List<PrefName> listPrefNames=new List<PrefName>() {
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
			_dictionaryClinicPrefsOld=ClinicPrefs.GetWhere(x => listPrefNames.Contains(x.PrefName))
				.GroupBy(x => x.ClinicNum)
				.ToDictionary(x => x.Key,x => x.ToList());
			_dictionaryClinicPrefsNew=_dictionaryClinicPrefsOld.ToDictionary(x => x.Key,x => x.Value.Select(y => y.Clone()).ToList());
			//Originally all ClinicPrefs were inserted together, so you either had none or all.
			//We have since added new ClincPrefs to this list so we need to identify missing clinicPrefs
			//Missing ClincPrefs will default to the standard preference table value.
			foreach(long clinicNum in _dictionaryClinicPrefsNew.Keys) {
				List<PrefName> listExistingClinicPrefs=_dictionaryClinicPrefsNew[clinicNum].Select(x => x.PrefName).ToList();
				List<PrefName> listMissingClincPrefs=listPrefNames.FindAll(x => !listExistingClinicPrefs.Contains(x));
				foreach(PrefName prefName in listMissingClincPrefs) {
					switch(EnumTools.GetAttributeOrDefault<PrefNameAttribute>(prefName).ValueType) {
						case PrefValueType.BOOL:
							bool defaultBool=PrefC.GetBool(prefName);
							_dictionaryClinicPrefsNew[clinicNum].Add(new ClinicPref(clinicNum,prefName,defaultBool));
						break;
						case PrefValueType.ENUM:
							//Currently not used.
						break;
						case PrefValueType.STRING:
							string defaultStr=PrefC.GetString(prefName);
							_dictionaryClinicPrefsNew[clinicNum].Add(new ClinicPref(clinicNum,prefName,defaultStr));
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
				|| !_dictionaryClinicPrefsNew.ContainsKey(listClinicNums[0]))//They have not saved their default filter options for the selected clinic. Use default prefs.
			{
				checkIncludeChanged.Checked=PrefC.GetBool(PrefName.BillingIncludeChanged);
				#region BillTypes
				listBillType.ClearSelected();
				string[] stringArrayselectedBillTypes=PrefC.GetString(PrefName.BillingSelectBillingTypes).Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries);
				for(int i = 0;i<stringArrayselectedBillTypes.Length;++i) { 
					try{
						int order=Defs.GetOrder(DefCat.BillingTypes,Convert.ToInt64(stringArrayselectedBillTypes[i]));
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
				List<ClinicPref> listClinicPrefs=_dictionaryClinicPrefsNew[listClinicNums[0]];//By definition of how ClinicPrefs are created, First will always return a result.
				checkIncludeChanged.Checked=PIn.Bool(listClinicPrefs.First(x => x.PrefName==PrefName.BillingIncludeChanged).ValueString);
				#region BillTypes
				listBillType.ClearSelected();
				string[] stringArraySelectedBillTypes=listClinicPrefs.First(x => x.PrefName==PrefName.BillingSelectBillingTypes).ValueString.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries);
				for(int i = 0;i<stringArraySelectedBillTypes.Length;++i) {
					try {
						int order=Defs.GetOrder(DefCat.BillingTypes,Convert.ToInt64(stringArraySelectedBillTypes[i]));
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
				selectedBillingTypes=string.Join(",",listBillType.SelectedIndices.OfType<int>().Select(x => _listDefsBillingType[x-1].DefNum));
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
				MsgBox.Show(this,"Settings saved.");
				return;
			}
			else if(_dictionaryClinicPrefsNew.Keys.Contains(listClinicNums[0])) {//ClincPrefs exist, update them.
				_dictionaryClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingIncludeChanged).ValueString=POut.Bool(checkIncludeChanged.Checked);
				_dictionaryClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingSelectBillingTypes).ValueString=selectedBillingTypes;
				_dictionaryClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingAgeOfAccount).ValueString=ageOfAccount;
				_dictionaryClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingExcludeBadAddresses).ValueString=POut.Bool(checkBadAddress.Checked);
				_dictionaryClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingExcludeInactive).ValueString=POut.Bool(checkExcludeInactive.Checked);
				_dictionaryClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingExcludeNegative).ValueString=POut.Bool(!checkShowNegative.Checked);
				_dictionaryClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingExcludeInsPending).ValueString=POut.Bool(checkExcludeInsPending.Checked);
				_dictionaryClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingExcludeIfUnsentProcs).ValueString=POut.Bool(checkExcludeIfProcs.Checked);
				_dictionaryClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingExcludeLessThan).ValueString=textExcludeLessThan.Text;
				_dictionaryClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingIgnoreInPerson).ValueString=POut.Bool(checkIgnoreInPerson.Checked);
				_dictionaryClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingShowTransSinceBalZero).ValueString=POut.Bool(checkBoxBillShowTransSinceZero.Checked);
				_dictionaryClinicPrefsNew[listClinicNums[0]].First(x => x.PrefName==PrefName.BillingDefaultsNote).ValueString=textNote.Text;
			}
			else {//No existing ClinicPrefs for the currently selected clinic in comboClinics.
				_dictionaryClinicPrefsNew.Add(listClinicNums[0],new List<ClinicPref>());
				//Inserts new ClinicPrefs for each of the Filter options for the currently selected clinic.
				_dictionaryClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingIncludeChanged,checkIncludeChanged.Checked));
				_dictionaryClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingSelectBillingTypes,selectedBillingTypes));
				_dictionaryClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingAgeOfAccount,ageOfAccount));
				_dictionaryClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingExcludeBadAddresses,checkBadAddress.Checked));
				_dictionaryClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingExcludeInactive,checkExcludeInactive.Checked));
				_dictionaryClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingExcludeNegative,!checkShowNegative.Checked));
				_dictionaryClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingExcludeInsPending,checkExcludeInsPending.Checked));
				_dictionaryClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingExcludeIfUnsentProcs,checkExcludeIfProcs.Checked));
				_dictionaryClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingExcludeLessThan,textExcludeLessThan.Text));
				_dictionaryClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingIgnoreInPerson,checkIgnoreInPerson.Checked));
				_dictionaryClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingShowTransSinceBalZero,checkBoxBillShowTransSinceZero.Checked));
				_dictionaryClinicPrefsNew[listClinicNums[0]].Add(new ClinicPref(listClinicNums[0],PrefName.BillingDefaultsNote,textNote.Text));
			}
			if(ClinicPrefs.Sync(_dictionaryClinicPrefsNew.SelectMany(x => x.Value).ToList(),_dictionaryClinicPrefsOld.SelectMany(x => x.Value).ToList())) {
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
				RefreshClinicPrefs();
			}
			MsgBox.Show(this,"Settings saved.");
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
			gridDunning.BeginUpdate();
			gridDunning.ListGridColumns.Clear();
			GridColumn column=new GridColumn("Billing Type",80);
			gridDunning.ListGridColumns.Add(column);
			column=new GridColumn("Aging",70);
			gridDunning.ListGridColumns.Add(column);
			column=new GridColumn("Ins",40);
			gridDunning.ListGridColumns.Add(column);
			column=new GridColumn("Message",150);
			gridDunning.ListGridColumns.Add(column);
			column=new GridColumn("Bold Message",150);
			gridDunning.ListGridColumns.Add(column);
			column=new GridColumn("Email",30, HorizontalAlignment.Center);
			gridDunning.ListGridColumns.Add(column);
			gridDunning.ListGridRows.Clear();
			GridRow row;
			for(int i = 0;i<_listDunnings.Count;++i) { 
				row=new GridRow();
				if(_listDunnings[i].BillingType==0){
					row.Cells.Add(Lan.g(this,"all"));
				}
				else{
					row.Cells.Add(Defs.GetName(DefCat.BillingTypes,_listDunnings[i].BillingType));
				}
				if(_listDunnings[i].AgeAccount==0){
					row.Cells.Add(Lan.g(this,"any"));
				}
				else{
					row.Cells.Add(Lan.g(this,"Over ")+_listDunnings[i].AgeAccount.ToString());
				}
				if(_listDunnings[i].InsIsPending==YN.Yes) {
					row.Cells.Add(Lan.g(this,"Y"));
				}
				else if(_listDunnings[i].InsIsPending==YN.No) {
					row.Cells.Add(Lan.g(this,"N"));
				}
				else {//YN.Unknown
					row.Cells.Add(Lan.g(this,"any"));
				}
				row.Cells.Add(_listDunnings[i].DunMessage);
				row.Cells.Add(new GridCell(_listDunnings[i].MessageBold) { Bold=YN.Yes,ColorText=Color.DarkRed });
				if(_listDunnings[i].EmailBody!="" || _listDunnings[i].EmailSubject!="") {
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}
				gridDunning.ListGridRows.Add(row);
			}
			gridDunning.EndUpdate();
		}

		private void gridDun_CellDoubleClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			using FormDunningEdit formDunningEdit=new FormDunningEdit(_listDunnings[e.Row]);
			formDunningEdit.ShowDialog();
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
			using FormDunningSetup formDunningSetup=new FormDunningSetup();
			formDunningSetup.ShowDialog();
			FillDunning();
		}

		private void butDefaults_Click(object sender,EventArgs e) {
			using FormBillingDefaults formBillingDefaults=new FormBillingDefaults();
			formBillingDefaults.ShowDialog();
			if(formBillingDefaults.DialogResult==DialogResult.OK){
				SetDefaults();
			}
		}

		private void SetDefaults(){
			textDateStart.Text=DateTime.Today.AddDays(-PrefC.GetLong(PrefName.BillingDefaultsLastDays)).ToShortDateString();
			textDateEnd.Text=DateTime.Today.ToShortDateString();
			checkIntermingled.Checked=PrefC.GetBool(PrefName.BillingDefaultsIntermingle);
			checkSinglePatient.Checked=PrefC.GetBool(PrefName.BillingDefaultsSinglePatient);
			if(SmsPhones.IsIntegratedTextingEnabled()) {
				string[] stringArrayBillingDefaultsModesToText=PrefC.GetString(PrefName.BillingDefaultsModesToText)
					.Split(new string[] { "," },StringSplitOptions.RemoveEmptyEntries);
				for(int i = 0;i<stringArrayBillingDefaultsModesToText.Length;++i) {
					listModeToText.SetSelected(PIn.Int(stringArrayBillingDefaultsModesToText[i]),true);
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
			DateTime dateTimeNow=MiscData.GetNowDateTime();
			DateTime dateTimeToday=dateTimeNow.Date;
			DateTime dateTimeLastAging=PrefC.GetDate(PrefName.DateLastAging);
			if(dateTimeLastAging.Date==dateTimeToday) {
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
			Prefs.UpdateString(PrefName.AgingBeginDateTime,POut.DateT(dateTimeNow,false));//get lock on pref to block others
			Signalods.SetInvalid(InvalidType.Prefs);//signal a cache refresh so other computers will have the updated pref as quickly as possible
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				Ledgers.ComputeAging(0,dateTimeToday);
				Prefs.UpdateString(PrefName.DateLastAging,POut.Date(dateTimeToday,false));
			};
			progressOD.StartingMessage=Lan.g(this,"Calculating enterprise aging for all patients as of")+" "+dateTimeToday.ToShortDateString()+"...";
			progressOD.MessageCancel=Lan.g(this,"You should not Cancel because this might leave some or all patient balances set to 0. If you do cancel, make sure to run Aging again. Cancel anyway?");
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
		private StatementMode GetStatementMode(PatAging patAging) {
			StatementMode statementMode;
			if(ListTools.In(PrefC.GetInt(PrefName.BillingUseElectronic),1,2,3,4)) {
				statementMode=StatementMode.Electronic;
			}
			else {
				statementMode=StatementMode.Mail;
			}
			Def billingType=Defs.GetDef(DefCat.BillingTypes,patAging.BillingType);
			if(billingType != null && billingType.ItemValue=="E") {
				statementMode=StatementMode.Email;
			}
			return statementMode;
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
				using FormAging formAging=new FormAging();
				formAging.BringToFront();
				formAging.ShowDialog();
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
			using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(_popUpMessage);
			msgBoxCopyPaste.Text=Lan.g(this,"Billing List Results");
			msgBoxCopyPaste.ShowDialog();
			return countAllClinics;
		}

		///<summary>Creates statements for a single clinic.  Returns number of statements created, or -1 if there was an error or user cancelled.</summary>
		///<param name="clinicNum">The clinic that billing is being generated for. Passing in a value of -2 indicates clinics not being enabled.</param>
		///<param name="useClinicPrefs">Indicates to override what is shown in the UI and use the defaul values for a clinic.</param>
		///<param name="suppressPopup">Set to true to turn off any message boxes from appearing.</param>
		///<param name="dictionaryPatAgingData">A dictionary of key=PatNum, value=PatAgingData, which is an object that stores aging data for the patient
		///regardless of clinics. Pass in to avoid calculating these values from within this function.</param>
		private int CreateHelper(long clinicNum,bool useClinicPrefs=false,bool suppressPopup=false,Dictionary<long,PatAgingData> dictionaryPatAgingData=null) {
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
			if(comboAge.SelectedIndex==1) {
				getAge="30";
			}
			else if(comboAge.SelectedIndex==2) {
				getAge="60";
			}
			else if(comboAge.SelectedIndex==3) {
				getAge="90";
			}
			List<long> billingNums=new List<long>();//[listBillType.SelectedIndices.Count];
			for(int i=0;i<listBillType.SelectedIndices.Count;i++){
				if(listBillType.SelectedIndices[i]==0){//if (all) is selected, then ignore any other selections
					billingNums.Clear();
					break;
				}
				billingNums.Add(_listDefsBillingType[listBillType.SelectedIndices[i]-1].DefNum);
			}
			List<PatAging> listPatAgings=new List<PatAging>();
			if(dictionaryPatAgingData==null) {//If not passed in, we must generate the data here.
				dictionaryPatAgingData=AgingData.GetAgingData(checkSinglePatient.Checked,checkIncludeChanged.Checked,checkExcludeInsPending.Checked,
					checkExcludeIfProcs.Checked,checkSuperFam.Checked,listClinicNums);
			}
			//This method was frequently causing a Middle Tier error. Grab all information, filtered by clinic if clinics enabled, and use the
			//PatAgingData dictionary to create a list of PatNums with unsent procs, a list of PatNums with pending ins, and a dictionary of PatNum key to
			//last trans date value and send only that data as it's all that GetAgingList needs.
			List<long> listPendingInsPatNums=new List<long>();
			List<long> listUnsentPatNums=new List<long>();
			SerializableDictionary<long,List<PatAgingTransaction>> dictionaryPatAgingTransactions=new SerializableDictionary<long,List<PatAgingTransaction>>();
			if(checkExcludeInsPending.Checked || checkExcludeIfProcs.Checked || checkIncludeChanged.Checked) {
				foreach(KeyValuePair<long,PatAgingData> kvp in dictionaryPatAgingData) {
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
						dictionaryPatAgingTransactions[kvp.Key]=kvp.Value.ListPatAgingTransactions;
					}
				}
			}
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => listPatAgings=Patients.GetAgingList(getAge,lastStatement,billingNums,checkBadAddress.Checked,!checkShowNegative.Checked,
					PIn.Double(textExcludeLessThan.Text),checkExcludeInactive.Checked,checkIgnoreInPerson.Checked,listClinicNums,checkSuperFam.Checked,
					checkSinglePatient.Checked,listPendingInsPatNums,listUnsentPatNums,dictionaryPatAgingTransactions);
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
			List<Patient> listPatientsSuperHeads=new List<Patient>();
			//If making a super family bill, we need to manipulate the agingList to only contain super family head members.
			//It can also contain regular family members, but should not contain any individual super family members other than the head.
			if(checkSuperFam.Checked) {
				List<PatAging> listPatAgingsSuperFamilies=new List<PatAging>();
				for(int i=listPatAgings.Count-1;i>=0;i--) {//Go through each PatAging in the retrieved list
					if(listPatAgings[i].SuperFamily==0 || !listPatAgings[i].HasSuperBilling) {
						continue;//It is okay to leave non-super billing PatAgings in the list.
					}
					Patient patientSuperFamilyHead=listPatientsSuperHeads.FirstOrDefault(x => x.PatNum==listPatAgings[i].SuperFamily);
					if(patientSuperFamilyHead==null) {
						patientSuperFamilyHead=Patients.GetPat(listPatAgings[i].SuperFamily);
						listPatientsSuperHeads.Add(patientSuperFamilyHead);
					}
					if(!patientSuperFamilyHead.HasSuperBilling) {
						listPatAgings[i].HasSuperBilling=false;//Family guarantor has super billing but superhead doesn't, so no super bill.  Mark statement as no superbill.
						continue;
					}
					//If the guar has super billing enabled and the superhead has superbilling, this entry needs to be merged to superbill.
					if(listPatAgings[i].HasSuperBilling && patientSuperFamilyHead.HasSuperBilling) {
						PatAging patAging=listPatAgingsSuperFamilies.FirstOrDefault(x => x.PatNum==patientSuperFamilyHead.PatNum);//Attempt to find an existing PatAging for the superhead.
						if(patAging==null) {
							//Create new PatAging object using SuperHead "credentials" but the guarantor's balance information.
							patAging=new PatAging();
							patAging.AmountDue=listPatAgings[i].AmountDue;
							patAging.BalTotal=listPatAgings[i].BalTotal;
							patAging.Bal_0_30=listPatAgings[i].Bal_0_30;
							patAging.Bal_31_60=listPatAgings[i].Bal_31_60;
							patAging.Bal_61_90=listPatAgings[i].Bal_61_90;
							patAging.BalOver90=listPatAgings[i].BalOver90;
							patAging.InsEst=listPatAgings[i].InsEst;
							patAging.PatName=patientSuperFamilyHead.GetNameLF();
							patAging.DateLastStatement=listPatAgings[i].DateLastStatement;
							patAging.BillingType=patientSuperFamilyHead.BillingType;
							patAging.PayPlanDue=listPatAgings[i].PayPlanDue;
							patAging.PatNum=patientSuperFamilyHead.PatNum;
							patAging.HasSuperBilling=listPatAgings[i].HasSuperBilling;//true
							patAging.SuperFamily=listPatAgings[i].SuperFamily;//Same as superHead.PatNum
							patAging.ClinicNum=listPatAgings[i].ClinicNum;
							listPatAgingsSuperFamilies.Add(patAging);
						}
						else {
							//Sum the information together for all guarantors of the superfamily.
							patAging.AmountDue+=listPatAgings[i].AmountDue;
							patAging.BalTotal+=listPatAgings[i].BalTotal;
							patAging.Bal_0_30+=listPatAgings[i].Bal_0_30;
							patAging.Bal_31_60+=listPatAgings[i].Bal_31_60;
							patAging.Bal_61_90+=listPatAgings[i].Bal_61_90;
							patAging.BalOver90+=listPatAgings[i].BalOver90;
							patAging.InsEst+=listPatAgings[i].InsEst;
							patAging.PayPlanDue+=listPatAgings[i].PayPlanDue;
						}
						listPatAgings.RemoveAt(i);//Remove the individual guarantor statement from the aging list since it's been combined with a superstatement.
					}
				}
				listPatAgings.AddRange(listPatAgingsSuperFamilies);
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
			if(listPatAgings.Count==0){
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
				_popUpMessage+=Lan.g(this,"Statements created")+": "+POut.Int(listPatAgings.Count)+"\r\n";
			}
			#endregion
			IsHistoryStartMinDate=string.IsNullOrWhiteSpace(textDateStart.Text);
			DateTime dateRangeFrom=PIn.Date(textDateStart.Text);
			DateTime dateRangeTo=DateTime.Today;//Needed for payplan accuracy.//new DateTime(2200,1,1);
			if(textDateEnd.Text!=""){
				dateRangeTo=PIn.Date(textDateEnd.Text);
			}
			Statement statement;
			DateTime dateAsOf=DateTime.Today;//used to determine when the balance on this date began
			if(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)) {//if aging calculated monthly, use the last aging date instead of today
				dateAsOf=PrefC.GetDate(PrefName.DateLastAging);
			}
			List<WebServiceMainHQProxy.ShortGuidResult> listShortGuidUrls=new List<WebServiceMainHQProxy.ShortGuidResult>();
			SheetDef sheetDefStatement=SheetUtil.GetStatementSheetDef();
			if(//They are going to send texts
				listModeToText.SelectedIndices.Count > 0 
				//Or the email body has a statement URL
				|| PrefC.GetString(PrefName.BillingEmailBodyText).ToLower().Contains("[statementurl]")
				|| PrefC.GetString(PrefName.BillingEmailBodyText).ToLower().Contains("[statementshorturl]")
				//Or the statement sheet has a URL field
				|| (sheetDefStatement!=null && sheetDefStatement.SheetFieldDefs.Any(x => x.FieldType==SheetFieldType.OutputText 
							&& (x.FieldValue.ToLower().Contains("[statementurl]") || x.FieldValue.ToLower().Contains("[statementshorturl]"))))) 
			{
				//Then get some short GUIDs and URLs from OD HQ.
				try {
					int countForSms=listPatAgings.Count(x => DoSendSms(x,dictionaryPatAgingData));
					//Previously we were reserving more guids then neccessary. Would just dump extra GUIDS.
					listShortGuidUrls=WebServiceMainHQProxy.GetShortGUIDs(countForSms,countForSms,clinicNum,eServiceCode.PatientPortalViewStatement);
				}
				catch(Exception ex) {
					FriendlyException.Show(Lans.g(this,"Unable to create a unique URL for each statement. The Patient Portal URL will be used instead."),ex);
					listShortGuidUrls=listPatAgings.Select(x => new WebServiceMainHQProxy.ShortGuidResult {
						MediumURL=PrefC.GetString(PrefName.PatientPortalURL),
						ShortURL=PrefC.GetString(PrefName.PatientPortalURL),
						ShortGuid=""
					}).ToList();
				}
			}
			Dictionary<long,InstallmentPlan> dictionaryInstallmentPlans=InstallmentPlans.GetForFams(listPatAgings.Select(x => x.PatNum).ToList());
			List<Statement> listStatementsForInsert=new List<Statement>();
			DateTime dateBalBeganCur;
			for(int i = 0;i<listPatAgings.Count;++i) {
				statement=new Statement();
				statement.DateRangeFrom=dateRangeFrom;
				statement.DateRangeTo=dateRangeTo;
				statement.DateSent=DateTime.Today;
				statement.DocNum=0;
				statement.HidePayment=false;
				statement.Intermingled=checkIntermingled.Checked;
				statement.IsSent=false;
				statement.Mode_=GetStatementMode(listPatAgings[i]);
				if(ListTools.In(PrefC.GetInt(PrefName.BillingUseElectronic),1,2,3,4)) {
					statement.Intermingled=true;
				}
				bool doSendSms=DoSendSms(listPatAgings[i],dictionaryPatAgingData);
				statement.SmsSendStatus=(doSendSms ? AutoCommStatus.SendNotAttempted : AutoCommStatus.DoNotSend);
				statement.ShortGUID="";
				statement.StatementURL="";
				statement.StatementShortURL="";
				WebServiceMainHQProxy.ShortGuidResult shortGuidUrl=listShortGuidUrls.FirstOrDefault(x => x.IsForSms==doSendSms);
				if(shortGuidUrl!=null) {
					statement.ShortGUID=shortGuidUrl.ShortGuid;
					statement.StatementURL=shortGuidUrl.MediumURL;
					statement.StatementShortURL=shortGuidUrl.ShortURL;
					listShortGuidUrls.Remove(shortGuidUrl);
				}
				statement.Note=textNote.Text;
				InstallmentPlan installmentPlan;
				if(dictionaryInstallmentPlans.TryGetValue(listPatAgings[i].PatNum,out installmentPlan) && installmentPlan!=null) {
					statement.Note=textNote.Text.Replace("[InstallmentPlanTerms]","Installment Plan\r\n"
						+"Date First Payment: "+installmentPlan.DateFirstPayment.ToShortDateString()+"\r\n"
						+"Monthly Payment: "+installmentPlan.MonthlyPayment.ToString("c")+"\r\n"
						+"APR: "+(installmentPlan.APR/100).ToString("P")+"\r\n"
						+"Note: "+installmentPlan.Note);
				}
				PatAgingData patAgingData;
				dictionaryPatAgingData.TryGetValue(listPatAgings[i].PatNum,out patAgingData);
				//appointment reminders are not handled here since it would be too slow.
				DateTime dateBalZeroCur=patAgingData?.DateBalZero??DateTime.MinValue;//dateBalZeroCur will be DateTime.MinValue if PatNum isn't in dict
				if(checkBoxBillShowTransSinceZero.Checked && dateBalZeroCur.Year > 1880) {
					statement.DateRangeFrom=dateBalZeroCur;
				}
				//dateBalBegan is first transaction date for a charge that consumed the last of the credits for the account, so first transaction that isn't
				//fully paid for based on oldest paid first logic
				dateBalBeganCur=patAgingData?.DateBalBegan??DateTime.MinValue;//dateBalBeganCur will be DateTime.MinValue if PatNum isn't in dict
				int ageAccount=0;
				//ageAccount is number of days between the day the account first started to have a positive bal and the asOf date
				if(dateBalBeganCur>DateTime.MinValue) {
					ageAccount=(dateAsOf-dateBalBeganCur).Days;
				}
				List<Dunning> listDunningsForPat=_listDunnings.FindAll(x => (x.BillingType==0 || x.BillingType==listPatAgings[i].BillingType) //same billing type
					&& x.ClinicNum==listPatAgings[i].ClinicNum
					&& ageAccount>=x.AgeAccount-x.DaysInAdvance //old enough to qualify for this dunning message, taking into account DaysInAdvance
					&& (x.InsIsPending==YN.Unknown || x.InsIsPending==(listPatAgings[i].InsEst>0?YN.Yes:YN.No)));//dunning msg ins pending=unkown or matches this acct
				Dunning dunning=listDunningsForPat.LastOrDefault(x => !x.IsSuperFamily);
				if(listPatAgings[i].HasSuperBilling && listPatAgings[i].PatNum==listPatAgings[i].SuperFamily && checkSuperFam.Checked) {
					dunning=listDunningsForPat.LastOrDefault(x => x.IsSuperFamily);
					statement.SuperFamily=listPatAgings[i].SuperFamily;//If this bill is for the superhead and has superbill enabled, it's a superbill.  Flag it as such.
				}
				if(dunning!=null){
					if(statement.Note!=""){
						statement.Note+="\r\n\r\n";//leave one empty line
					}
					statement.Note+=dunning.DunMessage;
					statement.NoteBold=dunning.MessageBold;
					statement.EmailSubject=dunning.EmailSubject;					
					statement.EmailBody=dunning.EmailBody;
				}
				statement.PatNum=listPatAgings[i].PatNum;
				statement.SinglePatient=checkSinglePatient.Checked;
				statement.IsBalValid=true;
				statement.BalTotal=listPatAgings[i].BalTotal;
				statement.InsEst=listPatAgings[i].InsEst;
				listStatementsForInsert.Add(statement);
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
