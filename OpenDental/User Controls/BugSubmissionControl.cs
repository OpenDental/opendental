using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.UI {
	public partial class BugSubmissionControl:UserControl {
		///<summary>Currently selected bugsubmission, either passed in from the calling form or from the group seleciton made in the Customer/Grouping grid.</summary>
		private BugSubmission _subCur;
		///<summary>The patient associated to _subCur</summary>
		private Patient _patCur;
		///<summary>Controls if the Customer/Grouping grid is used.</summary>
		private BugSubmissionControlMode _controlMode;
		///<summary>Passed in when grouping logic needs to be considered.</summary>
		private int _groupSelection;
		///<summary>List of bug submissions to be used in teh Customer/Grouping grid. Logic is dependent on _groupSelection value.</summary>
		private List<BugSubmission> _listSubs;
		///<summary>Fires when the user leaves the TextDevNote textbox.</summary>
		public TextDevNote_PostLeave TextDevNoteLeave;
		///<summary>Dictionary of patients that will lazy load as users click on entries.  The key is the Registration Key.</summary>
		private Dictionary<string,Patient> _dictPatients;
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		///<summary>Fires when the user clicks the customer subs grid.</summary>
		public OnGridCustomerSubs_CellClick OnGridCustomerSubsCellClick;
		private static Color _colorGridHeaderBack=Color.FromArgb(223,234,245);
		
		public BugSubmissionControlMode ControlMode {
			get {
				return _controlMode;
			}
			set {
				_controlMode=value;
			}
		}

		public BugSubmissionControl() {
			InitializeComponent();
		}
		
		private void BugSubmissionControl_Load(object sender,EventArgs e) {
			switch(_controlMode) {
				case BugSubmissionControlMode.Specific:
					LayoutManager.MoveHeight(gridOfficeInfo,gridOfficeInfo.Height+((gridCustomerSubs.Location.Y-gridOfficeInfo.Bottom)+gridCustomerSubs.Height));
					gridCustomerSubs.Visible=false;
					break;
				case BugSubmissionControlMode.General:
					break;
			}
		}

		///<summary>Must be called before RefreshView(...) to set the internal data.</summary>
		public void RefreshData(Dictionary<string,Patient> dictPatients,int groupSelection,List<BugSubmission> listSubs) {
			_dictPatients=(dictPatients==null?new Dictionary<string, Patient>():dictPatients);
			_groupSelection=groupSelection;
			_listSubs=listSubs;
		}

		/// <summary>Used to refresh the view to show the given sub information. Make sure and call RefreshData(...) when data has refreshed.</summary>
		/// <param name="sub">The bugSubmission that will be used to fill the UI.</param>
		/// <param name="pat">The bugSubmissions assocated patient, used for linking task and UI.</param>
		/// <param name="groupSelection">When using the control to view many bugSubmissions this is the grouping selection.</param>
		/// <param name="listSubs">The list of bugSubmissions to be considered in teh Customer/Grouping grid based on the groupSelection value.</param>
		public void RefreshView(BugSubmission sub,bool isCustomerGridRefresh=true) {
			_subCur=sub;
			_patCur=(_dictPatients.ContainsKey(_subCur.RegKey)?_dictPatients[_subCur.RegKey]:new Patient());
			RefreshViews();
			FillOfficeInfoGrid(_subCur);
			SetCustomerInfo(_subCur,isCustomerGridRefresh);
		}

		private void RefreshViews() {
			textStack.Text=_subCur.ExceptionStackTrace;
			textODStack.Text=_subCur.OdStackSignature;
			textStrippedStack.Text=_subCur.SimplifiedStackTrace;
			textDevNote.Text=_subCur.DevNote;
			listBoxCategories.Items.Clear();
			listBoxCategories.Items.AddList(_subCur.ListCategoryTags,x => x.ToString());
			butAddCategory.Enabled=true;
			butDeleteCategory.Enabled=true;
		}
		
		private void FillOfficeInfoGrid(BugSubmission sub){
			gridOfficeInfo.BeginUpdate();
			gridOfficeInfo.ListGridRows.Clear();
			if(sub==null) {
				gridOfficeInfo.EndUpdate();
				return;
			}
			if(gridOfficeInfo.ListGridColumns.Count==0) {
				gridOfficeInfo.ListGridColumns.Add(new GridColumn("Field",130));
				gridOfficeInfo.ListGridColumns.Add(new GridColumn("Value",125));
			}
			if(sub.IsMobileSubmission) {
				gridOfficeInfo.ListGridRows.Add(new GridRow("Mobile Fields","") { ColorBackG=_colorGridHeaderBack,Bold=true,Tag=true });
				gridOfficeInfo.ListGridRows.Add(new GridRow("AppTarget",sub.Info.AppTarget));
				gridOfficeInfo.ListGridRows.Add(new GridRow("EConnectorVersion",sub.Info.EConnectorVersion));
				gridOfficeInfo.ListGridRows.Add(new GridRow("AppVersion",sub.Info.AppVersion));
				gridOfficeInfo.ListGridRows.Add(new GridRow("DevicePlatform",sub.Info.DevicePlatform));
				gridOfficeInfo.ListGridRows.Add(new GridRow("DeviceModel",sub.Info.DeviceModel));
				gridOfficeInfo.ListGridRows.Add(new GridRow("DeviceVersion",sub.Info.DeviceVersion));
				gridOfficeInfo.ListGridRows.Add(new GridRow("DeviceManufacturer",sub.Info.DeviceManufacturer));
				gridOfficeInfo.ListGridRows.Add(new GridRow("MWUserIdentifier",sub.Info.MWUserIdentifier.ToString()));
				gridOfficeInfo.ListGridRows.Add(new GridRow("TimeSignalsLastReceived",sub.Info.TimeSignalsLastReceived.ToString()));
				gridOfficeInfo.ListGridRows.Add(new GridRow("DeviceId",sub.Info.DeviceId));
			}
			else {
				gridOfficeInfo.ListGridRows.Add(new GridRow("Preferences","") { ColorBackG=_colorGridHeaderBack,Bold=true,Tag=true });
				List<PrefName> listPrefNames=sub.Info.DictPrefValues.Keys.ToList();
				foreach(PrefName prefName in listPrefNames) {
					GridRow row=new GridRow();
					row.Cells.Add(prefName.ToString());
					row.Cells.Add(sub.Info.DictPrefValues[prefName]);
					gridOfficeInfo.ListGridRows.Add(row);
				}
				gridOfficeInfo.ListGridRows.Add(new GridRow("Other","") { ColorBackG=_colorGridHeaderBack,Bold=true,Tag=true });
				gridOfficeInfo.ListGridRows.Add(new GridRow("CountClinics",sub.Info.CountClinics.ToString()));
				gridOfficeInfo.ListGridRows.Add(new GridRow("EnabledPlugins",string.Join(",",sub.Info.EnabledPlugins?.Select(x => x).ToList()??new List<string>())));
				gridOfficeInfo.ListGridRows.Add(new GridRow("ClinicNumCur",sub.Info.ClinicNumCur.ToString()));
				gridOfficeInfo.ListGridRows.Add(new GridRow("UserNumCur",sub.Info.UserNumCur.ToString()));
				gridOfficeInfo.ListGridRows.Add(new GridRow("PatientNumCur",sub.Info.PatientNumCur.ToString()));
				gridOfficeInfo.ListGridRows.Add(new GridRow("ModuleNameCur",sub.Info.ModuleNameCur?.ToString()));
				gridOfficeInfo.ListGridRows.Add(new GridRow("IsOfficeOnReplication",sub.Info.IsOfficeOnReplication.ToString()));
				gridOfficeInfo.ListGridRows.Add(new GridRow("IsOfficeUsingMiddleTier",sub.Info.IsOfficeUsingMiddleTier.ToString()));
				gridOfficeInfo.ListGridRows.Add(new GridRow("WindowsVersion",sub.Info.WindowsVersion?.ToString()));
				gridOfficeInfo.ListGridRows.Add(new GridRow("CompName",sub.Info.CompName?.ToString()));
				gridOfficeInfo.ListGridRows.Add(new GridRow("ServerName",sub.Info.ServerName?.ToString()));
				gridOfficeInfo.ListGridRows.Add(new GridRow("PreviousUpdateVersion",sub.Info.PreviousUpdateVersion));
				gridOfficeInfo.ListGridRows.Add(new GridRow("PreviousUpdateTime",sub.Info.PreviousUpdateTime.ToString()));
				gridOfficeInfo.ListGridRows.Add(new GridRow("ThreadName",sub.Info.ThreadName?.ToString()));
				gridOfficeInfo.ListGridRows.Add(new GridRow("DatabaseName",sub.Info.DatabaseName?.ToString()));
				gridOfficeInfo.ListGridRows.Add(new GridRow("StorageEngine",sub.Info.StorageEngine?.ToString()));
				gridOfficeInfo.ListGridRows.Add(new GridRow("ODBdllVersion",sub.Info.OpenDentBusinessVersion?.ToString()));
				gridOfficeInfo.ListGridRows.Add(new GridRow("ODBdllMTVersion",sub.Info.OpenDentBusinessMiddleTierVersion?.ToString()??"INVALID"));
				gridOfficeInfo.ListGridRows.Add(new GridRow("EnvSessionName",sub.Info.EnvSessionName));
			}
			#region Shared between mobile and proper 
			gridOfficeInfo.ListGridRows.Add(new GridRow("ClinicNumCur",sub.Info.ClinicNumCur.ToString()));
			#endregion
			gridOfficeInfo.EndUpdate();
		}

		///<summary>When sub is set, fills customer group box with various information.
		///When null, clears all fields.</summary>
		private void SetCustomerInfo(BugSubmission sub,bool isCustomerGridRefresh=true) {
			try {
				labelCustomerNum.Text=_patCur?.PatNum.ToString()??"";
				labelRegKey.Text=sub.RegKey;
				labelCustomerState.Text=_patCur?.State??"";
				labelCustomerPhone.Text=_patCur?.WkPhone??"";
				labelSubNum.Text=POut.Long(sub.BugSubmissionNum);
				labelLastCall.Text=Commlogs.GetDateTimeOfLastEntryForPat(_patCur?.PatNum??0).ToString();
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			butGoToAccount.Enabled=true;
			butBugTask.Enabled=true;
			butCompare.Enabled=true;
			if(!isCustomerGridRefresh || _groupSelection==-1 || _listSubs==null || !gridCustomerSubs.Visible) {//Just in case checks.
				return;
			}
			switch(_groupSelection) {
				case 0:
					#region None
					gridCustomerSubs.Title="Customer Submissions";
					gridCustomerSubs.BeginUpdate();
					gridCustomerSubs.ListGridColumns.Clear();
					gridCustomerSubs.ListGridColumns.Add(new GridColumn("Version",100,HorizontalAlignment.Center));
					gridCustomerSubs.ListGridColumns.Add(new GridColumn("Count",50,HorizontalAlignment.Center));
					gridCustomerSubs.ListGridRows.Clear();
					Dictionary<string,List<BugSubmission>> dictCustomerSubVersions=_listSubs
						.Where(x => x.RegKey==sub.RegKey)
						.GroupBy(x => x.TryGetPrefValue(PrefName.ProgramVersion,"0.0.0.0"))
						.ToDictionary(x => x.Key,x => x.DistinctBy(y => y.ExceptionStackTrace).ToList());
					foreach(KeyValuePair<string,List<BugSubmission>> pair in dictCustomerSubVersions) {
						gridCustomerSubs.ListGridRows.Add(new GridRow(pair.Key,pair.Value.Count.ToString()));
					}
					gridCustomerSubs.EndUpdate();
					#endregion
					break;
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
					#region RegKey/Ver/Stack, Stacktrace, 95%, StackSig
					gridCustomerSubs.Title="Grouped Subs ("+_listSubs.DistinctBy(x => x.RegKey).Count()+" Reg)";
					if(ListTools.In(_groupSelection,4,5)) {//StackSig,StackSimple
						gridCustomerSubs.Title+=" ("+_listSubs.DistinctBy(x => x.ExceptionStackTrace).Count()+" Stacks)";
					}
					gridCustomerSubs.BeginUpdate();
					gridCustomerSubs.ListGridColumns.Clear();
					gridCustomerSubs.ListGridColumns.Add(new GridColumn("Vers.",55,HorizontalAlignment.Center));
					gridCustomerSubs.ListGridColumns.Add(new GridColumn("RegKey",60,HorizontalAlignment.Center){ IsWidthDynamic=true });
					gridCustomerSubs.ListGridRows.Clear();
					_listSubs.ForEach(x => { 
						Patient rowPat;
						string patDescript=x.RegKey;
						if(_dictPatients.TryGetValue(x.RegKey,out rowPat)) {
							patDescript=rowPat.GetNameLF();
						}
						GridRow row=new GridRow(x.TryGetPrefValue(PrefName.ProgramVersion,"0.0.0.0"),patDescript);
						row.Tag=x;
						gridCustomerSubs.ListGridRows.Add(row);
					});
					gridCustomerSubs.EndUpdate();
					#endregion
					break;
			}
		}

		public void ClearCustomerInfo() {
			textStack.Text="";//Also clear any submission specific fields.
			textODStack.Text="";
			textStrippedStack.Text="";
			labelCustomerNum.Text="";
			labelRegKey.Text="";
			labelCustomerState.Text="";
			labelCustomerPhone.Text="";
			labelSubNum.Text="";
			labelLastCall.Text="";
			FillOfficeInfoGrid(null);
			gridCustomerSubs.BeginUpdate();
			gridCustomerSubs.ListGridRows.Clear();
			gridCustomerSubs.EndUpdate();
			butGoToAccount.Enabled=false;
			butBugTask.Enabled=false;
			butCompare.Enabled=false;
			textDevNote.Text="";
			listBoxCategories.Items.Clear();
			butAddCategory.Enabled=false;
			butDeleteCategory.Enabled=false;
		}
		
		public void SetTextDevNoteEnabled(bool value) {
			this.textDevNote.Enabled=value;
		}
		
		private void textDevNote_Leave(object sender,EventArgs e) {
			if(_subCur.DevNote==textDevNote.Text) {
				return;
			}
			BugSubmission subOld=_subCur.Copy();
			_subCur.DevNote=textDevNote.Text;
			BugSubmissions.Update(_subCur,subOld);
			TextDevNoteLeave?.Invoke(sender,e);
		}
		
		private void butAddCategory_Click(object sender,EventArgs e) {
			using InputBox input=new InputBox("Please enter a category tag");
			if(input.ShowDialog()!=DialogResult.OK) {
				return;
			}
			BugSubmission subOld=_subCur.Copy();
			List<string> listCats=subOld.ListCategoryTags;
			string categoryNew=input.ListTextEntered[0];
			listCats.Add(categoryNew);
			_subCur.CategoryTags=string.Join(",",listCats);
			BugSubmissions.Update(_subCur,subOld);
			RefreshViews();
		}
		
		private void butDeleteCategory_Click(object sender,EventArgs e) {
			int index=listBoxCategories.SelectedIndex;
			if(index==-1) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"You are about to delete this category, continue?")) {
				return;
			}
			BugSubmission subOld=_subCur.Copy();
			List<string> listCats=subOld.ListCategoryTags;
			listCats.RemoveAt(index);
			_subCur.CategoryTags=string.Join(",",listCats);
			BugSubmissions.Update(_subCur,subOld);
			RefreshViews();
		}

		private void gridCustomerSubs_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Row==-1 || _groupSelection==0) {//0=None
				_subCur=null;
				return;
			}
			_subCur=(BugSubmission)gridCustomerSubs.ListGridRows[e.Row].Tag;
			_patCur=(_dictPatients.ContainsKey(_subCur.RegKey)?_dictPatients[_subCur.RegKey]:new Patient());
			RefreshView(_subCur,false);
			OnGridCustomerSubsCellClick?.Invoke(_subCur);
		}
		
		private void gridCustomerSubs_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(e.Row==-1 || _groupSelection==0) {//0=None
				return;
			}
			using FormBugSubmissions formGroupBugSubs=new FormBugSubmissions(formBugSubmissionMode:FormBugSubmissionMode.ViewOnly);
			formGroupBugSubs.ListBugSubmissionsViewed=_listSubs;
			formGroupBugSubs.ShowDialog();
		}

		private void butGoToAccount_Click(object sender,EventArgs e) {
			if(_patCur==null) {
				return;
			}
			//Button is only enabled if _patCur is not null.
			GotoModule.GotoAccount(_patCur.PatNum);
		}

		private void butBugTask_Click(object sender,EventArgs e) {
			if(_patCur==null) {
				return;
			}
			BugSubmissionL.CreateTask(_patCur,_subCur);
		}
		
		private void butCompare_Click(object sender,EventArgs e) {
			using InputBox input=new InputBox("Please copy/paste your stack trace to compare to this bug.",true);
			if(input.ShowDialog()!=DialogResult.OK) {
				return;
			}
			string perct=POut.Double(BugSubmissionL.CalculateSimilarity(textStack.Text,input.textResult.Text));
			MsgBox.Show(this,perct+"%");
		}
		
		public delegate void TextDevNote_PostLeave(object sender,EventArgs e);
		public delegate void OnGridCustomerSubs_CellClick(BugSubmission sub);

	}

}

public enum BugSubmissionControlMode {
	Specific,
	General
}
