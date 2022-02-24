using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormTsiHistory:FormODBase {
		private List<long> _listSelectedFamPatNums;//to find messages sent for a patient who is no longer the guarantor of the family
		private List<Clinic> _listClinics;
		private List<TsiTransLog> _listTsiTransLogsAll;
		private Dictionary<long,Patient> _dictPatLims;
		private TsiTransLog _selectedLog;

		public FormTsiHistory() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTsiHistory_Load(object sender,EventArgs e) {
			SetFilterControlsAndAction(() => FillGrid(),datePicker,checkShowPatNums);
			#region Fill Clinics
			_listClinics=new List<Clinic>();
			if(PrefC.HasClinicsEnabled) {
				_listClinics.AddRange(
					Clinics.GetForUserod(Security.CurUser,true).OrderBy(x => x.ClinicNum!=0).ThenBy(x => x.ItemOrder)
				);
			}
			else {//clinics disabled
				_listClinics.Add(Clinics.GetPracticeAsClinicZero(Lan.g(this,"Unassigned")));
			}
			#endregion Fill Clinics
			#region Fill Client IDs
			comboClientIDs.IncludeAll=true;
			comboClientIDs.IsAllSelected=true;
			List<string> listClientIDs=new List<string>();
			long progNum=Programs.GetProgramNum(ProgramName.Transworld);
			if(progNum>0) {
				listClientIDs=ProgramProperties.GetWhere(x => x.ProgramNum==progNum && ListTools.In(x.PropertyDesc,"ClientIdAccelerator","ClientIdCollection"))
					.Select(x => x.PropertyValue).Distinct().ToList();
			}
			comboClientIDs.Items.AddList(listClientIDs,x => x);
			#endregion Fill Client IDs
			#region Fill Trans Types
			List<TsiTransType> listTransTypes=Enum.GetValues(typeof(TsiTransType)).OfType<TsiTransType>().ToList();
			comboTransTypes.IncludeAll=true;
			comboTransTypes.IsAllSelected=true;
			comboTransTypes.Items.AddList(listTransTypes,x => x.GetDescription());
			#endregion Fill Trans Types
			#region Fill Account Statuses
			List<string> listAcctStatuses=new[] { "Active","Suspended","Inactive" }.ToList();
			comboAcctStatuses.IncludeAll=true;
			comboAcctStatuses.IsAllSelected=true;
			comboAcctStatuses.Items.AddList(listAcctStatuses,x => x);
			#endregion Fill Account Statuses
			#region Get Selected Family PatNums
			_listSelectedFamPatNums=new List<long>();
			if(FormOpenDental.CurPatNum>0) {
				Family fam=Patients.GetFamily(FormOpenDental.CurPatNum);
				textPatient.Text=fam.GetNameInFamLF(FormOpenDental.CurPatNum);
				_listSelectedFamPatNums=fam.ListPats.Select(x => x.PatNum).ToList();
			}
			#endregion Get Selected Family PatNums
			#region Get All TsiTransLogs
			_listTsiTransLogsAll=TsiTransLogs.GetAll();
			#endregion Get All TsiTransLogs
			#region Add ClientIDs No Longer In Use
			List<string> listLogClientIDs=_listTsiTransLogsAll.Select(x => x.ClientId).Distinct().Where(x => !listClientIDs.Contains(x)).ToList();
			foreach(string clientIDCur in listLogClientIDs) {
				listClientIDs.Add(clientIDCur);
				comboClientIDs.Items.Add(clientIDCur+" (no longer used)",clientIDCur);
			}
			#endregion Add ClientIDs No Longer In Use
			_dictPatLims=Patients.GetLimForPats(_listTsiTransLogsAll.Select(x => x.PatNum).Distinct().ToList()/*,true*/).ToDictionary(x => x.PatNum);
			datePicker.SetDateTimeFrom(DateTime.Today.AddDays(-1));//Set start to yesterday
			datePicker.SetDateTimeTo(DateTime.Today);//Set stop date to today to limit the number of messages that load immediately
			_selectedLog=null;
			FillGrid();
		}

		private void FillGrid() {
			Cursor=Cursors.WaitCursor;
			datePicker.HideCalendars();
			List<int> listLogIndexesFiltered=GetListIndexesFiltered();
			#region Set Grid Columns
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn("Guarantor",PrefC.HasClinicsEnabled?200:300){ IsWidthDynamic=true,DynamicWeight=PrefC.HasClinicsEnabled?20:30 });
			if(PrefC.HasClinicsEnabled) {
				gridMain.ListGridColumns.Add(new GridColumn("Clinic",100){ IsWidthDynamic=true,DynamicWeight=10 });
			}
			gridMain.ListGridColumns.Add(new GridColumn("User",100){ IsWidthDynamic=true,DynamicWeight=10 });
			gridMain.ListGridColumns.Add(new GridColumn("Trans Type",80){ IsWidthDynamic=true,DynamicWeight=10 });
			gridMain.ListGridColumns.Add(new GridColumn("Trans Date Time",120,HorizontalAlignment.Center,GridSortingStrategy.DateParse){ IsWidthDynamic=true,DynamicWeight=12 });
			gridMain.ListGridColumns.Add(new GridColumn("Service Type",85){ IsWidthDynamic=true,DynamicWeight=8.5f });
			gridMain.ListGridColumns.Add(new GridColumn("Service Code",85){ IsWidthDynamic=true,DynamicWeight=8.5f });
			gridMain.ListGridColumns.Add(new GridColumn("Client ID",75){ IsWidthDynamic=true,DynamicWeight=7.5f });
			gridMain.ListGridColumns.Add(new GridColumn("Trans Amt",70,HorizontalAlignment.Right,GridSortingStrategy.AmountParse){ IsWidthDynamic=true,DynamicWeight=7 });
			gridMain.ListGridColumns.Add(new GridColumn("Account Bal",80,HorizontalAlignment.Right,GridSortingStrategy.AmountParse){ IsWidthDynamic=true,DynamicWeight=8 });
			gridMain.ListGridColumns.Add(new GridColumn("Key Type",100){ IsWidthDynamic=true,DynamicWeight=10 });
			#endregion Set Grid Columns
			#region Fill Grid Rows
			List<Patient> listFilteredPatLims=listLogIndexesFiltered.Where(x => _dictPatLims.ContainsKey(_listTsiTransLogsAll[x].PatNum))//Just in case.
				.Select(x => _dictPatLims[_listTsiTransLogsAll[x].PatNum]).DistinctBy(x => x.PatNum).ToList();
			Dictionary<long,string> dictPatNames=listFilteredPatLims.ToDictionary(x => x.PatNum,x => x.GetNameLF());
			Dictionary<long,string> dictClinicAbbrs=_listClinics.Where(x => x.ClinicNum>0).ToDictionary(x => x.ClinicNum,x => x.Abbr);
			Dictionary<long,string> dictPatClinicAbbrs=listFilteredPatLims.ToDictionary(x => x.PatNum,x => Clinics.GetAbbr(x.ClinicNum));
			Dictionary<long,string> dictUserNames=Userods.GetUsers(listLogIndexesFiltered.Select(x => _listTsiTransLogsAll[x].UserNum).Distinct().ToList())
				.ToDictionary(x => x.UserNum,x => x.UserName);
			gridMain.ListGridRows.Clear();
			int rowToReselect=-1;
			GridRow row;
			foreach(int i in listLogIndexesFiltered) {
				TsiTransLog logCur=_listTsiTransLogsAll[i];
				row=new GridRow();
				string patName="";
				row.Cells.Add((checkShowPatNums.Checked?(logCur.PatNum.ToString()+" - "):"")
					+(dictPatNames.TryGetValue(logCur.PatNum,out patName)?patName:Patients.GetNameLF(logCur.PatNum)));
				if(PrefC.HasClinicsEnabled) {
					string clinicAbbr="";
					row.Cells.Add((dictClinicAbbrs.TryGetValue(logCur.ClinicNum,out clinicAbbr) || dictPatClinicAbbrs.TryGetValue(logCur.PatNum,out clinicAbbr))?clinicAbbr:"");
				}
				string userName="";
				row.Cells.Add(dictUserNames.TryGetValue(logCur.UserNum,out userName)?userName:"");
				row.Cells.Add(logCur.TransType.GetDescription());
				row.Cells.Add(logCur.TransDateTime.ToString("g"));
				row.Cells.Add(logCur.ServiceType.GetDescription());
				row.Cells.Add(logCur.ServiceCode.GetDescription());
				row.Cells.Add(logCur.ClientId);
				row.Cells.Add(logCur.TransAmt.ToString("n"));
				row.Cells.Add(logCur.AccountBalance.ToString("n"));
				row.Cells.Add(logCur.FKeyType.GetDescription());
				row.Tag=i;
				gridMain.ListGridRows.Add(row);
				if(_selectedLog!=null && _selectedLog.TsiTransLogNum==logCur.TsiTransLogNum) {
					rowToReselect=gridMain.ListGridRows.Count-1;
				}
			}
			#endregion Fill Grid Rows
			gridMain.EndUpdate();
			if(_selectedLog!=null) {//row was selected before call to FillGrid
				if(rowToReselect>-1) {
					gridMain.SetSelected(rowToReselect,true);
				}
				else {//row was selected but is no longer in the grid
					textSelectedFieldName.Clear();
					textSelectedFieldDetails.Clear();
					textRawMsg.SelectionChanged-=textRawMsg_SelectionChanged;
					textRawMsg.Clear();
					textRawMsg.SelectionChanged+=textRawMsg_SelectionChanged;
					_selectedLog=null;
				}
			}
			Cursor=Cursors.Default;
		}

		///<summary>Return a list of filtered indexes relative to the index in _listTsiTransLogsAll.
		///We use indexes to conserve memory.</summary>
		private List<int> GetListIndexesFiltered() {
			List<int> retval=new List<int>();
			#region Validate Filters
			ValidateChildren(ValidationConstraints.Enabled|ValidationConstraints.Visible|ValidationConstraints.Selectable);
			if(!datePicker.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return retval;//return empty list, filter inputs cannot be applied since there are errors
			}
			#endregion Validate Filters
			#region Get Filter Data
			DateTime dateStart=datePicker.GetDateTimeFrom();
			DateTime dateEnd=datePicker.GetDateTimeTo();
			dateEnd=(dateEnd==DateTime.MinValue?DateTime.Today:dateEnd);
			List<TsiTransType> listSelectedTransTypes=new List<TsiTransType>();
			if(!comboTransTypes.IsAllSelected) {
				listSelectedTransTypes=comboTransTypes.GetListSelected<TsiTransType>();
			}
			List<string> listSelectedAcctStatuses=new List<string>();
			if(!comboAcctStatuses.IsAllSelected) {
				listSelectedAcctStatuses=comboAcctStatuses.GetListSelected<string>();
			}
			List<long> listSuspendedGuarNums=TsiTransLogs.GetSuspendedGuarNums();
			List<long> listActiveCollGuarNums=Patients.GetListCollectionGuarNums(/*false*/);
			Dictionary<long,long> dictLogClinic=new Dictionary<long,long>();
			if(PrefC.HasClinicsEnabled) {
				dictLogClinic=_listTsiTransLogsAll
					.ToDictionary(x => x.TsiTransLogNum,x => x.ClinicNum>0?x.ClinicNum:(_dictPatLims.TryGetValue(x.PatNum,out Patient pat)?pat.ClinicNum:0));
			}
			List<string> listClientIDs=new List<string>();
			if(!comboClientIDs.IsAllSelected) {
				listClientIDs=comboClientIDs.GetListSelected<string>();
			}
			#endregion Get Filter Data
			#region Apply Filter Data
			for(int i=0;i<_listTsiTransLogsAll.Count;i++) {
				TsiTransLog logCur=_listTsiTransLogsAll[i];
				if(logCur.TransDateTime.Date<=dateEnd
					&& logCur.TransDateTime.Date>=dateStart
					&& (_listSelectedFamPatNums.Count==0 || _listSelectedFamPatNums.Contains(logCur.PatNum))
					&& (comboClinics.ListSelectedClinicNums.Count==0 || (dictLogClinic.ContainsKey(logCur.TsiTransLogNum) && comboClinics.ListSelectedClinicNums.Contains(dictLogClinic[logCur.TsiTransLogNum])))
					&& (listClientIDs.Count==0 || listClientIDs.Contains(logCur.ClientId))
					&& (listSelectedTransTypes.Count==0 || listSelectedTransTypes.Contains(logCur.TransType))
					&& (listSelectedAcctStatuses.Count==0
						|| (listSelectedAcctStatuses.Contains("Active") && listActiveCollGuarNums.Contains(logCur.PatNum))
						|| (listSelectedAcctStatuses.Contains("Suspended") && listSuspendedGuarNums.Contains(logCur.PatNum))
						|| (listSelectedAcctStatuses.Contains("Inactive") && !listActiveCollGuarNums.Concat(listSuspendedGuarNums).Contains(logCur.PatNum))))
				{
					retval.Add(i);
				}
			}
			#endregion Apply Filter Data
			return retval;
		}

		private void gridMain_ColumnSorted(object sender,EventArgs e) {
			if(_selectedLog==null) {
				return;
			}
			//SetSelected handles -1, so safely handles the case of item not found with selected TsiTransLogNum
			gridMain.SetSelected(gridMain.ListGridRows.ToList()
				.FindIndex(x => x.Tag is int
					&& (int)x.Tag>=0
					&& (int)x.Tag<_listTsiTransLogsAll.Count
					&& _listTsiTransLogsAll[(int)x.Tag].TsiTransLogNum==_selectedLog.TsiTransLogNum)
				,true);
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Row<0
				|| e.Row>=gridMain.ListGridRows.Count
				|| !(gridMain.ListGridRows[e.Row].Tag is int)
				|| (int)gridMain.ListGridRows[e.Row].Tag<0
				|| (int)gridMain.ListGridRows[e.Row].Tag>=_listTsiTransLogsAll.Count)
			{
				return;
			}
			_selectedLog=_listTsiTransLogsAll[(int)gridMain.ListGridRows[e.Row].Tag];
			textSelectedFieldName.Clear();
			textSelectedFieldDetails.Clear();
			textRawMsg.SelectionChanged-=textRawMsg_SelectionChanged;
			textRawMsg.Clear();
			textRawMsg.Text=_selectedLog.RawMsgText;
			if(_selectedLog.TransType!=TsiTransType.None) {
				textRawMsg.SelectionChanged+=textRawMsg_SelectionChanged;
			}
			if(_selectedLog.TransType==TsiTransType.Agg) {//When an aggregate row is selected, then also highlight the rows which belong to the aggregate.
				//Rows which belong to the aggregate should all be visible, since the date/time for all the rows should be within 1 second of each other.
				for(int i=0;i<gridMain.ListGridRows.Count;i++) {
					TsiTransLog tsiTransLogCur=_listTsiTransLogsAll[(int)gridMain.ListGridRows[i].Tag];
					if(tsiTransLogCur.AggTransLogNum==_selectedLog.TsiTransLogNum) {
						gridMain.SetSelected(i,true);
					}
				}
			}
		}

		private void textRawMsg_SelectionChanged(object sender,EventArgs e) {			
			textSelectedFieldName.Clear();
			textSelectedFieldDetails.Clear();
			if(gridMain.GetSelectedIndex()<0 || gridMain.GetSelectedIndex()>=gridMain.ListGridRows.Count || !(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag is int)) {
				return;//because gridMain.SelectedTag will return default(T), which is 0 for type int and we don't want to use 0 unless it's actually selected
			}
			int selectedLogIndex=gridMain.SelectedTag<int>();
			if(selectedLogIndex<0 || selectedLogIndex>_listTsiTransLogsAll.Count) {
				return;
			}
			TsiTransLog selectedLog=_listTsiTransLogsAll[selectedLogIndex];
			int selectionStart=textRawMsg.SelectionStart;
			int selectionLength=textRawMsg.SelectionLength;
			if(selectionStart<0 || selectionStart>textRawMsg.Text.Length) {
				return;
			}
			string msgHeader="";
			if(selectedLog.TransType==TsiTransType.PL) {
				msgHeader=TsiMsgConstructor.GetPlacementFileHeader();
			}
			else {
				msgHeader=TsiMsgConstructor.GetUpdateFileHeader();
			}
			if(string.IsNullOrWhiteSpace(msgHeader)) {
				return;
			}
			string[] fieldDescripts=msgHeader.Split('|');
			int countPipesToLeftOfCursor=textRawMsg.Text.Substring(0,selectionStart).Count(x => x=='|');
			if(countPipesToLeftOfCursor<fieldDescripts.Length) {
				textSelectedFieldName.Text=fieldDescripts[countPipesToLeftOfCursor];
			}
			int selectedFieldStartIndex=textRawMsg.Text.Substring(0,selectionStart).LastIndexOf('|')+1;
			int selectedFieldEndIndex=textRawMsg.Text.IndexOf('|',selectionStart);
			selectedFieldEndIndex=(selectedFieldEndIndex==-1?textRawMsg.Text.Length:selectedFieldEndIndex);
			RichTextBox rtfTemp=new RichTextBox();
			rtfTemp.Text=textRawMsg.Text;//rtfTemp will have the same text without any rtf formatting
			if(selectedFieldStartIndex>0) {
				rtfTemp.Select(selectedFieldStartIndex-1,1);
				rtfTemp.SelectionFont=new Font(rtfTemp.SelectionFont,FontStyle.Bold);
			}
			if(selectedFieldEndIndex<rtfTemp.Text.Length) {
				rtfTemp.Select(selectedFieldEndIndex,1);
				rtfTemp.SelectionFont=new Font(rtfTemp.SelectionFont,FontStyle.Bold);
			}
			if(selectedFieldStartIndex<selectedFieldEndIndex) {
				rtfTemp.Select(selectedFieldStartIndex,selectedFieldEndIndex-selectedFieldStartIndex);
				rtfTemp.SelectionColor=Color.Red;//select and change the color in the temp textbox
			}
			if(countPipesToLeftOfCursor<fieldDescripts.Length) {
				FillTextSelectedFieldDetails(fieldDescripts[countPipesToLeftOfCursor],rtfTemp.SelectedText);
			}
			textRawMsg.SelectionChanged-=textRawMsg_SelectionChanged;
			textRawMsg.Rtf=rtfTemp.Rtf;//set the textbox rtf to the temp textbox rtf to color the text red
			textRawMsg.Select(selectionStart,selectionLength);
			textRawMsg.SelectionChanged+=textRawMsg_SelectionChanged;
		}

		private void FillTextSelectedFieldDetails(string fieldDescript,string selectedText) {
			switch(fieldDescript) {
				case "CLIENT NUMBER":
					textSelectedFieldDetails.Text="ID assigned to the office/clinic by TSI";
					return;
				case "TRANSMITTAL NUMBER":
					textSelectedFieldDetails.Text="Not used";
					return;
				case "DEBTOR REFERENCE":
				case "TRANSMITTAL/REFERENCE NUMBER":
					textSelectedFieldDetails.Text="Open Dental PatNum for the guarantor of the family";
					return;
				case "DEBTOR PHONE":
					textSelectedFieldDetails.Text="Home phone for the guarantor of the family";
					return;
				case "SECONDARY PHONE":
					textSelectedFieldDetails.Text="Wireless phone for the guarantor of the family";
					return;
				case "PATIENTTYPE":
					if(selectedText=="0") {
						textSelectedFieldDetails.Text="Self/Default";
					}
					else if(selectedText=="1") {
						textSelectedFieldDetails.Text="Spouse/Significant other";
					}
					else if(selectedText=="2") {
						textSelectedFieldDetails.Text="Parent/Guardian";
					}
					else if(selectedText=="3") {
						textSelectedFieldDetails.Text="Child";
					}
					else if(selectedText=="4") {
						textSelectedFieldDetails.Text="Other (Grandparent/Grandchild/Sitter/Sibling/Friend/Other)";
					}
					return;
				case "PATIENTPHONE1":
					textSelectedFieldDetails.Text="Home phone for the patient being placed for account management by TSI";
					return;
				case "PATIENTPHONE2":
					textSelectedFieldDetails.Text="Wireless phone for the patient being placed for account management by TSI";
					return;
				case "SERVICETYPE":
					if(selectedText=="1") {
						textSelectedFieldDetails.Text="Accelerator/Profit Recovery";
					}
					else if(selectedText=="2") {
						textSelectedFieldDetails.Text="Collection";
					}
					return;
				case "SERVICECODE":
					if(string.IsNullOrWhiteSpace(selectedText)) {
						return;//nothing in the field so no text selected for translation
					}
					textSelectedFieldDetails.Text=PIn.Enum<TsiServiceCode>(PIn.Int(selectedText,false)-1).GetDescription();
					return;
				case "TRANSACTION TYPE":
					if(string.IsNullOrWhiteSpace(selectedText)) {
						return;//nothing in the field so no text selected for translation
					}
					TsiTransType typeCur=PIn.Enum<TsiTransType>(selectedText,true);
					textSelectedFieldDetails.Text=typeCur.GetDescription();
					if(typeCur==TsiTransType.None) {
						textSelectedFieldDetails.Text+=" (transaction NOT sent to TSI, e.g. a payment received from TSI)";
					}
					return;
				case "DEBTOR DATE OF BIRTH":
				case "PATIENTDOB":
				case "DATE OF DEBT":
				case "DATE OF LAST PAY":
				case "TRANSACTION DATE":
					DateTime dateCur;
					if(string.IsNullOrWhiteSpace(selectedText)
						|| !DateTime.TryParseExact(selectedText,"MMddyyyy",new CultureInfo("en-US"),DateTimeStyles.AllowWhiteSpaces,out dateCur))
					{
						return;
					}
					textSelectedFieldDetails.Text=dateCur.ToShortDateString();
					return;
			}
		}

		private void ComboClinics_SelectionChangeCommitted(object sender, EventArgs e){
			FillGrid();
		}

		private void comboBoxMultiClientIDs_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboClientIDs.SelectedIndices.Count==0) {
				comboClientIDs.IsAllSelected=true;
			}
			FillGrid();
		}

		private void comboBoxMultiTransTypes_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboTransTypes.SelectedIndices.Count==0) {
				comboTransTypes.IsAllSelected=true;
			}
			FillGrid();
		}

		private void comboBoxMultiAcctStatuses_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboAcctStatuses.SelectedIndices.Count==0) {
				comboAcctStatuses.IsAllSelected=true;
			}
			FillGrid();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			_listTsiTransLogsAll=TsiTransLogs.GetAll();
			FillGrid();
		}

		private void butCurrent_Click(object sender,EventArgs e) {
			if(FormOpenDental.CurPatNum==0) {
				return;
			}
			Family fam=Patients.GetFamily(FormOpenDental.CurPatNum);
			textPatient.Text=fam.GetNameInFamLF(FormOpenDental.CurPatNum);
			_listSelectedFamPatNums=fam.ListPats.Select(x => x.PatNum).ToList();
			FillGrid();
		}

		private void butFind_Click(object sender,EventArgs e) {
			using FormPatientSelect FormPS=new FormPatientSelect();
			FormPS.SelectionModeOnly=true;
			if(FormPS.ShowDialog()!=DialogResult.OK) {
				return;
			}
			Family fam=Patients.GetFamily(FormPS.SelectedPatNum);
			textPatient.Text=fam.GetNameInFamLF(FormPS.SelectedPatNum);
			_listSelectedFamPatNums=fam.ListPats.Select(x => x.PatNum).ToList();
			FillGrid();
		}

		private void butAll_Click(object sender,EventArgs e) {
			if(_listSelectedFamPatNums.Count==0) {
				return;
			}
			_listSelectedFamPatNums=new List<long>();
			textPatient.Text="";
			FillGrid();
		}

		private void menuItemGoTo_Click(object sender,EventArgs e) {
			if(gridMain.SelectedGridRows.Count!=1) {
				MsgBox.Show(this,"Please select one TSI transaction log first.");
				return;
			}
			object logIndex=gridMain.SelectedGridRows[0].Tag;//Index is original location in _listTsiTransLogsAll
			if(!(logIndex is int) || (int)logIndex<0 || (int)logIndex>=_listTsiTransLogsAll.Count) {
				return;
			}
			//FormOpenDental.S_Contr_PatientSelected(Patients.GetPat(_listTsiTransLogsAll[(int)logIndex].PatNum),false);
			GotoModule.GotoAccount(_listTsiTransLogsAll[(int)logIndex].PatNum);
			SendToBack();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		
	}
}