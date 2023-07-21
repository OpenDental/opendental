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
		private List<long> _listPatNumsFam;//to find messages sent for a patient who is no longer the guarantor of the family
		private List<Clinic> _listClinics;
		private List<TsiTransLog> _listTsiTransLogs;
		private List<Patient> _listPatientsLim;
		private TsiTransLog _tsiTransLog;

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
				listClientIDs=ProgramProperties.GetWhere(x => x.ProgramNum==progNum && x.PropertyDesc.In("ClientIdAccelerator","ClientIdCollection"))
					.Select(x => x.PropertyValue).Distinct().ToList();
			}
			comboClientIDs.Items.AddList(listClientIDs,x => x);
			#endregion Fill Client IDs
			#region Fill Trans Types
			List<TsiTransType> listTsiTransTypes=Enum.GetValues(typeof(TsiTransType)).OfType<TsiTransType>().ToList();
			comboTransTypes.IncludeAll=true;
			comboTransTypes.IsAllSelected=true;
			comboTransTypes.Items.AddList(listTsiTransTypes,x => x.GetDescription());
			#endregion Fill Trans Types
			#region Fill Account Statuses
			List<string> listAcctStatuses=new[] { "Active","Suspended","Inactive" }.ToList();
			comboAcctStatuses.IncludeAll=true;
			comboAcctStatuses.IsAllSelected=true;
			comboAcctStatuses.Items.AddList(listAcctStatuses,x => x);
			#endregion Fill Account Statuses
			#region Get Selected Family PatNums
			_listPatNumsFam=new List<long>();
			if(FormOpenDental.PatNumCur>0) {
				Family family=Patients.GetFamily(FormOpenDental.PatNumCur);
				textPatient.Text=family.GetNameInFamLF(FormOpenDental.PatNumCur);
				_listPatNumsFam=family.ListPats.Select(x => x.PatNum).ToList();
			}
			#endregion Get Selected Family PatNums
			#region Get All TsiTransLogs
			_listTsiTransLogs=TsiTransLogs.GetAll();
			#endregion Get All TsiTransLogs
			#region Add ClientIDs No Longer In Use
			List<string> listLogClientIDs=_listTsiTransLogs.Select(x => x.ClientId).Distinct().Where(x => !listClientIDs.Contains(x)).ToList();
			for(int i = 0;i<listLogClientIDs.Count;i++) {
				listClientIDs.Add(listLogClientIDs[i]);
				comboClientIDs.Items.Add(listLogClientIDs[i]+" (no longer used)",listLogClientIDs[i]);
			}
			#endregion Add ClientIDs No Longer In Use
			
			_listPatientsLim=Patients.GetLimForPats(_listTsiTransLogs.Select(x => x.PatNum).Distinct().ToList(), doIncludeClinicNum:true);
			datePicker.SetDateTimeFrom(DateTime.Today.AddDays(-1));//Set start to yesterday
			datePicker.SetDateTimeTo(DateTime.Today);//Set stop date to today to limit the number of messages that load immediately
			_tsiTransLog=null;
			FillGrid();
		}

		private void FillGrid() {
			Cursor=Cursors.WaitCursor;
			datePicker.HideCalendars();
			List<int> listLogIndexesFiltered=GetListIndexesFiltered();
			#region Set Grid Columns
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn("Guarantor",PrefC.HasClinicsEnabled?200:300){ IsWidthDynamic=true,DynamicWeight=PrefC.HasClinicsEnabled?20:30 });
			if(PrefC.HasClinicsEnabled) {
				gridMain.Columns.Add(new GridColumn("Clinic",100){ IsWidthDynamic=true,DynamicWeight=10 });
			}
			gridMain.Columns.Add(new GridColumn("User",100){ IsWidthDynamic=true,DynamicWeight=10 });
			gridMain.Columns.Add(new GridColumn("Trans Type",80){ IsWidthDynamic=true,DynamicWeight=10 });
			gridMain.Columns.Add(new GridColumn("Trans Date Time",120,HorizontalAlignment.Center,GridSortingStrategy.DateParse){ IsWidthDynamic=true,DynamicWeight=12 });
			gridMain.Columns.Add(new GridColumn("Service Type",85){ IsWidthDynamic=true,DynamicWeight=8.5f });
			gridMain.Columns.Add(new GridColumn("Service Code",85){ IsWidthDynamic=true,DynamicWeight=8.5f });
			gridMain.Columns.Add(new GridColumn("Client ID",75){ IsWidthDynamic=true,DynamicWeight=7.5f });
			gridMain.Columns.Add(new GridColumn("Trans Amt",70,HorizontalAlignment.Right,GridSortingStrategy.AmountParse){ IsWidthDynamic=true,DynamicWeight=7 });
			gridMain.Columns.Add(new GridColumn("Account Bal",80,HorizontalAlignment.Right,GridSortingStrategy.AmountParse){ IsWidthDynamic=true,DynamicWeight=8 });
			gridMain.Columns.Add(new GridColumn("Key Type",100){ IsWidthDynamic=true,DynamicWeight=10 });
			#endregion Set Grid Columns
			#region Fill Grid Rows
			gridMain.ListGridRows.Clear();
			int rowToReselect=-1;
			GridRow row;
			for(int i=0;i<listLogIndexesFiltered.Count;i++) {
				TsiTransLog tsiTransLog=_listTsiTransLogs[listLogIndexesFiltered[i]];
				row=new GridRow();
				string patNum="";
				if (checkShowPatNums.Checked) {
					patNum=tsiTransLog.PatNum.ToString()+" - ";
				}
				string patName = _listPatientsLim.Find(x => x.PatNum==tsiTransLog.PatNum)?.GetNameLF();
				patName??=Patients.GetNameLF(tsiTransLog.PatNum);
				row.Cells.Add(patNum+patName);
				if(PrefC.HasClinicsEnabled) {
					string clinicAbbr=_listClinics.Find(x => x.ClinicNum==tsiTransLog.ClinicNum)?.Abbr;
					clinicAbbr??=Clinics.GetAbbr(_listPatientsLim.Find(x => x.PatNum==tsiTransLog.PatNum)?.ClinicNum??0);
					row.Cells.Add(clinicAbbr);
				}
				row.Cells.Add(Userods.GetName(tsiTransLog.UserNum));
				row.Cells.Add(tsiTransLog.TransType.GetDescription());
				row.Cells.Add(tsiTransLog.TransDateTime.ToString("g"));
				row.Cells.Add(tsiTransLog.ServiceType.GetDescription());
				row.Cells.Add(tsiTransLog.ServiceCode.GetDescription());
				row.Cells.Add(tsiTransLog.ClientId);
				row.Cells.Add(tsiTransLog.TransAmt.ToString("n"));
				row.Cells.Add(tsiTransLog.AccountBalance.ToString("n"));
				row.Cells.Add(tsiTransLog.FKeyType.GetDescription());
				row.Tag=i;
				gridMain.ListGridRows.Add(row);
				if(_tsiTransLog!=null && _tsiTransLog.TsiTransLogNum==tsiTransLog.TsiTransLogNum) {
					rowToReselect=gridMain.ListGridRows.Count-1;
				}
			}
			#endregion Fill Grid Rows
			gridMain.EndUpdate();
			if(_tsiTransLog==null) {//row was selected before call to FillGrid
				Cursor=Cursors.Default;
				return;
			}
			if(rowToReselect>-1) {
				gridMain.SetSelected(rowToReselect,setValue:true);
				Cursor=Cursors.Default;
				return;
			}
			//row was selected but is no longer in the grid
			textSelectedFieldName.Clear();
			textSelectedFieldDetails.Clear();
			textRawMsg.SelectionChanged-=textRawMsg_SelectionChanged;
			textRawMsg.Clear();
			textRawMsg.SelectionChanged+=textRawMsg_SelectionChanged;
			_tsiTransLog=null;
			Cursor=Cursors.Default;
		}

		///<summary>Return a list of filtered indexes relative to the index in _listTsiTransLogs.
		///We use indexes to conserve memory.</summary>
		private List<int> GetListIndexesFiltered() {
			List<int> listRetVals=new List<int>();
			#region Validate Filters
			ValidateChildren(ValidationConstraints.Enabled|ValidationConstraints.Visible|ValidationConstraints.Selectable);
			if(!datePicker.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return listRetVals;//return empty list, filter inputs cannot be applied since there are errors
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
			List<string> listClientIDs=new List<string>();
			if(!comboClientIDs.IsAllSelected) {
				listClientIDs=comboClientIDs.GetListSelected<string>();
			}
			#endregion Get Filter Data
			#region Apply Filter Data
			for(int i=0;i<_listTsiTransLogs.Count;i++) {
				long clinicNum=_listTsiTransLogs[i].ClinicNum;
				if(PrefC.HasClinicsEnabled && clinicNum<=0) {
					clinicNum = _listPatientsLim.Find(y=>y.PatNum==_listTsiTransLogs[i].PatNum)?.ClinicNum??0;
				}
				if(_listTsiTransLogs[i].TransDateTime.Date<=dateEnd
					&& _listTsiTransLogs[i].TransDateTime.Date>=dateStart
					&& (_listPatNumsFam.Count==0 || _listPatNumsFam.Contains(_listTsiTransLogs[i].PatNum))
					&& (comboClinics.ListSelectedClinicNums.Count==0 || comboClinics.ListSelectedClinicNums.Contains(clinicNum))
					&& (listClientIDs.Count==0 || listClientIDs.Contains(_listTsiTransLogs[i].ClientId))
					&& (listSelectedTransTypes.Count==0 || listSelectedTransTypes.Contains(_listTsiTransLogs[i].TransType))
					&& (listSelectedAcctStatuses.Count==0
						|| (listSelectedAcctStatuses.Contains("Active") && listActiveCollGuarNums.Contains(_listTsiTransLogs[i].PatNum))
						|| (listSelectedAcctStatuses.Contains("Suspended") && listSuspendedGuarNums.Contains(_listTsiTransLogs[i].PatNum))
						|| (listSelectedAcctStatuses.Contains("Inactive") && !listActiveCollGuarNums.Concat(listSuspendedGuarNums).Contains(_listTsiTransLogs[i].PatNum))))
				{
					listRetVals.Add(i);
				}
			}
			#endregion Apply Filter Data
			return listRetVals;
		}

		private void gridMain_ColumnSorted(object sender,EventArgs e) {
			if(_tsiTransLog==null) {
				return;
			}
			//SetSelected handles -1, so safely handles the case of item not found with selected TsiTransLogNum
			gridMain.SetSelected(gridMain.ListGridRows.ToList()
				.FindIndex(x => x.Tag is int
					&& (int)x.Tag>=0
					&& (int)x.Tag<_listTsiTransLogs.Count
					&& _listTsiTransLogs[(int)x.Tag].TsiTransLogNum==_tsiTransLog.TsiTransLogNum)
				,true);
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Row<0
				|| e.Row>=gridMain.ListGridRows.Count
				|| !(gridMain.ListGridRows[e.Row].Tag is int)
				|| (int)gridMain.ListGridRows[e.Row].Tag<0
				|| (int)gridMain.ListGridRows[e.Row].Tag>=_listTsiTransLogs.Count)
			{
				return;
			}
			_tsiTransLog=_listTsiTransLogs[(int)gridMain.ListGridRows[e.Row].Tag];
			textSelectedFieldName.Clear();
			textSelectedFieldDetails.Clear();
			textRawMsg.SelectionChanged-=textRawMsg_SelectionChanged;
			textRawMsg.Clear();
			textRawMsg.Text=_tsiTransLog.RawMsgText;
			if(_tsiTransLog.TransType!=TsiTransType.None) {
				textRawMsg.SelectionChanged+=textRawMsg_SelectionChanged;
			}
			if(_tsiTransLog.TransType!=TsiTransType.Agg) {
				return;
			}
			//When an aggregate row is selected, then also highlight the rows which belong to the aggregate.
			//Rows which belong to the aggregate should all be visible since the date/time for all the rows should be within 1 second of each other.
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				TsiTransLog tsiTransLog=_listTsiTransLogs[(int)gridMain.ListGridRows[i].Tag];
				if(tsiTransLog.AggTransLogNum==_tsiTransLog.TsiTransLogNum) {
					gridMain.SetSelected(i,true);
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
			if(selectedLogIndex<0 || selectedLogIndex>_listTsiTransLogs.Count) {
				return;
			}
			TsiTransLog tsiTransLogSelected=_listTsiTransLogs[selectedLogIndex];
			int selectionStart=textRawMsg.SelectionStart;
			int selectionLength=textRawMsg.SelectionLength;
			if(selectionStart<0 || selectionStart>textRawMsg.Text.Length) {
				return;
			}
			string msgHeader="";
			if(tsiTransLogSelected.TransType==TsiTransType.PL) {
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
			RichTextBox richTextBox_=new RichTextBox();
			richTextBox_.Text=textRawMsg.Text;//rtfTemp will have the same text without any rtf formatting
			if(selectedFieldStartIndex>0) {
				richTextBox_.Select(selectedFieldStartIndex-1,1);
				richTextBox_.SelectionFont=new Font(richTextBox_.SelectionFont,FontStyle.Bold);
			}
			if(selectedFieldEndIndex<richTextBox_.Text.Length) {
				richTextBox_.Select(selectedFieldEndIndex,1);
				richTextBox_.SelectionFont=new Font(richTextBox_.SelectionFont,FontStyle.Bold);
			}
			if(selectedFieldStartIndex<selectedFieldEndIndex) {
				richTextBox_.Select(selectedFieldStartIndex,selectedFieldEndIndex-selectedFieldStartIndex);
				richTextBox_.SelectionColor=Color.Red;//select and change the color in the temp textbox
			}
			if(countPipesToLeftOfCursor<fieldDescripts.Length) {
				FillTextSelectedFieldDetails(fieldDescripts[countPipesToLeftOfCursor],richTextBox_.SelectedText);
			}
			textRawMsg.SelectionChanged-=textRawMsg_SelectionChanged;
			textRawMsg.Rtf=richTextBox_.Rtf;//set the textbox rtf to the temp textbox rtf to color the text red
			textRawMsg.Select(selectionStart,selectionLength);
			textRawMsg.SelectionChanged+=textRawMsg_SelectionChanged;
		}

		private void FillTextSelectedFieldDetails(string fieldDescription,string selectedText) {
			switch(fieldDescription) {
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
					TsiTransType tsiTransType=PIn.Enum<TsiTransType>(selectedText,true);
					textSelectedFieldDetails.Text=tsiTransType.GetDescription();
					if(tsiTransType==TsiTransType.None) {
						textSelectedFieldDetails.Text+=" (transaction NOT sent to TSI, e.g. a payment received from TSI)";
					}
					return;
				case "DEBTOR DATE OF BIRTH":
				case "PATIENTDOB":
				case "DATE OF DEBT":
				case "DATE OF LAST PAY":
				case "TRANSACTION DATE":
					DateTime date;
					if(string.IsNullOrWhiteSpace(selectedText)
						|| !DateTime.TryParseExact(selectedText,"MMddyyyy",new CultureInfo("en-US"),DateTimeStyles.AllowWhiteSpaces,out date))
					{
						return;
					}
					textSelectedFieldDetails.Text=date.ToShortDateString();
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
			_listTsiTransLogs=TsiTransLogs.GetAll();
			FillGrid();
		}

		private void butCurrent_Click(object sender,EventArgs e) {
			if(FormOpenDental.PatNumCur==0) {
				return;
			}
			Family family=Patients.GetFamily(FormOpenDental.PatNumCur);
			textPatient.Text=family.GetNameInFamLF(FormOpenDental.PatNumCur);
			_listPatNumsFam=family.ListPats.Select(x => x.PatNum).ToList();
			FillGrid();
		}

		private void butFind_Click(object sender,EventArgs e) {
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.IsSelectionModeOnly=true;
			if(formPatientSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			Family family=Patients.GetFamily(formPatientSelect.PatNumSelected);
			textPatient.Text=family.GetNameInFamLF(formPatientSelect.PatNumSelected);
			_listPatNumsFam=family.ListPats.Select(x => x.PatNum).ToList();
			FillGrid();
		}

		private void butAll_Click(object sender,EventArgs e) {
			if(_listPatNumsFam.Count==0) {
				return;
			}
			_listPatNumsFam=new List<long>();
			textPatient.Text="";
			FillGrid();
		}

		private void menuItemGoTo_Click(object sender,EventArgs e) {
			if(gridMain.SelectedGridRows.Count!=1) {
				MsgBox.Show(this,"Please select one TSI transaction log first.");
				return;
			}
			object logIndex=gridMain.SelectedGridRows[0].Tag;//Index is original location in _listTsiTransLogsAll
			if(!(logIndex is int) || (int)logIndex<0 || (int)logIndex>=_listTsiTransLogs.Count) {
				return;
			}
			//FormOpenDental.S_Contr_PatientSelected(Patients.GetPat(_listTsiTransLogsAll[(int)logIndex].PatNum),false);
			GotoModule.GotoAccount(_listTsiTransLogs[(int)logIndex].PatNum);
			SendToBack();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}