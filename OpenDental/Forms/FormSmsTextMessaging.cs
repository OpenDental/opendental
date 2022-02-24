using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormSmsTextMessaging:FormODBase {

		#region Private data; properties/fields
		///<summary>Set from FormOpenDental.  This can be null if the calling code does not wish to get dynamic unread message counts.
		///Allows FormSmsTextMessaging to update the unread SMS text message count in FormOpenDental as the user reads their messages.</summary>
		private Action<int> _smsNotifier=null;
		///<summary>The selected patNum.  Can be -1 to include all.</summary>
		private long _patNumCur=-1;
		///<summary>The column index of the Status column within the Messages grid.
		///This is a class-wide variable to prevent bugs if we decide to change the column order of the Messages grid.</summary>
		private int _columnStatusIdx=0;
		private UserOdPref _groupByPref=null;
		private List<SmsFromMobile> _listSmsFromMobile=new List<SmsFromMobile>();
		private List<SmsToMobile> _listSmsToMobile=new List<SmsToMobile>();
		private Color _colorSelect=Color.FromArgb(224,243,255);
		private Dictionary<long,string> _dictPatNames=new Dictionary<long, string>();
		private DateTime _dateFrom {
			get {
				return PIn.Date(textDateFrom.Text);
			}
		}
		private DateTime _dateTo {
			get {
				return PIn.Date(textDateTo.Text);
			}
		}
		private bool _isGrouped {
			get {
				return radioGroupByPatient.Checked || radioGroupByPhone.Checked;
			}
		}
		private bool _isGroupedByPhone {
			get {
				return radioGroupByPhone.Checked;
			}
		}
		///<summary>Null if gridMessage current selected row tag is not TextMessageGrouped.</summary>
		private TextMessageGrouped _selectedSmsGroup {
			get {
				if(gridMessages.GetSelectedIndex()==-1) {
					return null;
				}
				object selectedTag=gridMessages.ListGridRows[gridMessages.GetSelectedIndex()].Tag;
				if(!(selectedTag is TextMessageGrouped)) {
					return null;
				}
				return (TextMessageGrouped)selectedTag;
			}
		}
		///<summary>Null if gridMessage current selected row tag is not SmsFromMobile.</summary>
		private SmsFromMobile _selectedSmsFromMobile {
			get {
				if(gridMessages.GetSelectedIndex()==-1) {
					return null;
				}
				object selectedTag=gridMessages.ListGridRows[gridMessages.GetSelectedIndex()].Tag;
				if(!(selectedTag is SmsFromMobile)) {
					return null;
				}
				return (SmsFromMobile)selectedTag;
			}
		}
		///<summary>Null if gridMessage current selected row tag is not SmsToMobile.</summary>
		private SmsToMobile _selectedSmsToMobile {
			get {
				if(gridMessages.GetSelectedIndex()==-1) {
					return null;
				}
				object selectedTag=gridMessages.ListGridRows[gridMessages.GetSelectedIndex()].Tag;
				if(!(selectedTag is SmsToMobile)) {
					return null;
				}
				return (SmsToMobile)selectedTag;
			}
		}

		private bool _hasSelectedMessage {
			get {
				return _selectedSmsGroup!=null || _selectedSmsFromMobile!=null || _selectedSmsToMobile!=null;
			}
		}

		///<summary>0 if gridMessage current selected row tag is not SmsToMobile or SmsFromMobile or TextMessageGrouped.</summary>
		private long _selectedPatNum {
			get {
				if(_selectedSmsGroup!=null) {
					return _selectedSmsGroup.PatNum;
				}
				if(_selectedSmsFromMobile!=null) {
					return _selectedSmsFromMobile.PatNum;
				}
				if(_selectedSmsToMobile!=null) {
					return _selectedSmsToMobile.PatNum;
				}
				return 0;
			}
		}

		///<summary>Empty if gridMessage current selected row tag is not SmsToMobile or SmsFromMobile or TextMessageGrouped.</summary>
		private string _selectedMobileNumber {
			get {
				if(_selectedSmsGroup!=null) {
					return _selectedSmsGroup.PatPhone;
				}
				if(_selectedSmsFromMobile!=null) {
					return _selectedSmsFromMobile.MobilePhoneNumber;
				}
				if(_selectedSmsToMobile!=null) {
					return _selectedSmsToMobile.MobilePhoneNumber;
				}
				return "";
			}
		}

		///<summary></summary>
		private List<long> GetListSelectedClinicNums() {
			if(!PrefC.HasClinicsEnabled) {
				return new List<long>();//An empty list will cause the clinic filter to be ignored in SmsFromMobiles.GetMessages()
			}
			//we don't want an empty list
			if(comboClinics.ListSelectedClinicNums.Count==0){
				return new List<long>(){Clinics.ClinicNum };//current clinic
			}
			return comboClinics.ListSelectedClinicNums;
		}

		///<summary>Always includes ReceivedUnread. May include ReceivedRead.</summary>
		private List<SmsFromStatus> _listSmsFromStatusesSelected {
			get {
				List<SmsFromStatus> ret=new List<SmsFromStatus>();
				ret.Add(SmsFromStatus.ReceivedUnread);
				if(checkRead.Checked) {
					ret.Add(SmsFromStatus.ReceivedRead);
				}
				return ret;
			}
		}

		///<summary>Passively load _dictPatNames from db if entry is not already found.</summary>
		private string GetPatientName(long patNum) {
			AddPatientNames(new List<long>() { patNum });
			return _dictPatNames.ContainsKey(patNum) ? _dictPatNames[patNum] : Lan.g(this,"Not found");
		}
		///<summary>Safe to call this for any number of PatNums. Will only go to db if given PatNum(s) not already found in _dictPatNames.</summary>
		private void AddPatientNames(List<long> listPatNums) {
			var newPatNums=listPatNums.Where(x => x!=0 && !_dictPatNames.ContainsKey(x)).ToList();
			if(newPatNums.Count()>0) {
				var newNames=Patients.GetPatientNames(newPatNums);
				foreach(var x in newNames) {
					_dictPatNames[x.Key]=x.Value;
				}
			}
		}
		#endregion

		#region Init
		public FormSmsTextMessaging(bool isSent,bool isReceived,Action<int> smsNotifier) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			checkSent.Checked=isSent;
			checkRead.Checked=isReceived;
			_smsNotifier=smsNotifier;
			_groupByPref=UserOdPrefs.GetFirstOrNewByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.SmsGroupBy);
			if(_groupByPref.ValueString=="1") {
				radioGroupByPatient.Checked=true;
			}
			else if(_groupByPref.ValueString=="2") {
				radioGroupByPhone.Checked=true;
			}
			else {
				radioGroupByNone.Checked=true;
			}
		}

		private void FormSmsTextMessaging_Load(object sender,EventArgs e) {
			gridMessages.ContextMenu=contextMenuMessages;
			if(PrefC.HasClinicsEnabled) {
				//comboClinics.HqDescription="Practice";
				//hqClinic.Abbr=(PrefC.GetString(PrefName.PracticeTitle)+" ("+Lan.g(this,"Practice")+")");
				comboClinics.SelectedClinicNum=Clinics.ClinicNum;
				if(PrefC.GetBool(PrefName.EnterpriseApptList)) {//This form behaves differently when compared to the other 6 forms
					comboClinics.IncludeAll=false;
				}
				else {
					if(comboClinics.SelectedClinicNum==0) {
						comboClinics.IsAllSelected=true;//jordan Users specifically requested this
					}
				}
			}
			textDateFrom.Text=DateTime.Today.AddDays(-7).ToShortDateString();
			textDateTo.Text=DateTime.Today.ToShortDateString();			
			FillGridTextMessages();
			//It is important that these events get registered after we have filled the form. Don't want them to fire twice when setting the initial values.
			radioGroupByNone.CheckedChanged+=new EventHandler(RadioGroupBy_CheckedChanged);
			radioGroupByPatient.CheckedChanged+=new EventHandler(RadioGroupBy_CheckedChanged);
			radioGroupByPhone.CheckedChanged+=new EventHandler(RadioGroupBy_CheckedChanged);
			this.FormClosing+=new FormClosingEventHandler((o,e1) => {
				if(radioGroupByNone.Checked) {
					_groupByPref.ValueString="0";
				}
				else if(radioGroupByPatient.Checked) {
					_groupByPref.ValueString="1";
				}
				else if(radioGroupByPhone.Checked) {
					_groupByPref.ValueString="2";
				}
				UserOdPrefs.Upsert(_groupByPref);
			});
			SetFilterControlsAndAction(() => SetMessageCounts(),0,textReply);
			Plugins.HookAddCode(this,"FormSmsTextMessaging.Load_end");
		}

		private void SetMessageCounts() {
			textCharCount.Text=textReply.TextLength.ToString();
			textMsgCount.Text=SmsPhones.CalculateMessagePartsNumber(textReply.Text).ToString();
		}

		private void RadioGroupBy_CheckedChanged(object sender,EventArgs e) {
			if(sender==null || !(sender is RadioButton) || !((RadioButton)sender).Checked) {
				return;
			}
			FillGridTextMessages(true,false);
		}
		#endregion

		#region Fill Grids
		private void FillGridTextMessages(bool isRedrawOnly=false,bool retainSort=true) {
			//Show/hide context menu items according to the grouping mode.
			new List<MenuItem>() {
				menuItemChangePat,
				menuItemMarkUnread,
				menuItemMarkRead,
				menuItemHide,
				menuItemUnhide,
				menuItemBlockNumber
			}
			.ForEach(x => x.Visible=!_isGrouped);
			if(PrefC.HasClinicsEnabled && comboClinics.ListSelectedClinicNums.Count==0) {
				gridMessages.BeginUpdate();
				gridMessages.ListGridRows.Clear();
				gridMessages.EndUpdate();
				return;
			}
			//Hold these. We will be clearing them below and they will need to be restored.
			int sortByColIdx=gridMessages.SortedByColumnIdx;
			bool isSortAsc=gridMessages.SortedIsAscending;
			if(sortByColIdx==-1 || !retainSort) {
				sortByColIdx=0;
				isSortAsc=false;
			}
			if(!isRedrawOnly) {
				_listSmsFromMobile=SmsFromMobiles.GetMessages(_dateFrom,_dateTo,GetListSelectedClinicNums(),_patNumCur,false,"",
					_listSmsFromStatusesSelected.ToArray());
				if(checkSent.Checked) {
					_listSmsToMobile=SmsToMobiles.GetMessages(_dateFrom,_dateTo,GetListSelectedClinicNums(),_patNumCur,"");
				}
				AddPatientNames(_listSmsFromMobile.GroupBy(x => x.PatNum).Select(x => x.Key)
					.Union(_listSmsToMobile.GroupBy(x => x.PatNum).Select(x => x.Key)).ToList());
			}
			if(_isGrouped) {
				FillGridTextMessagesGroupedYes(sortByColIdx,isSortAsc,_selectedPatNum,_selectedSmsGroup);
			}
			else {
				FillGridTextMessagesGroupedNo(sortByColIdx,isSortAsc,_selectedPatNum,_selectedSmsToMobile,_selectedSmsFromMobile);
			}
		}

		private void FillGridTextMessagesGroupedNo(int sortByColIdx,bool isSortAsc,long selectedPatNum,SmsToMobile selectedSmsToMobile,SmsFromMobile selectedSmsFromMobile) {
			gridMessages.Title=Lan.g(this,"Text Messages - Right click for options - Unread messages always shown");
			gridMessages.BeginUpdate();
			gridMessages.ListGridRows.Clear();
			gridMessages.ListGridColumns.Clear();
			gridMessages.ListGridColumns.Add(new UI.GridColumn("DateTime",140,HorizontalAlignment.Left) { SortingStrategy=UI.GridSortingStrategy.DateParse });
			gridMessages.ListGridColumns.Add(new UI.GridColumn("Sent /\r\nReceived",80,HorizontalAlignment.Center){SortingStrategy=UI.GridSortingStrategy.StringCompare} );
			gridMessages.ListGridColumns.Add(new UI.GridColumn("Status",70,HorizontalAlignment.Center) { SortingStrategy=UI.GridSortingStrategy.StringCompare });
			_columnStatusIdx=gridMessages.ListGridColumns.Count-1;
			gridMessages.ListGridColumns.Add(new UI.GridColumn("#Phone\r\nMatches",60,HorizontalAlignment.Center) { SortingStrategy=UI.GridSortingStrategy.AmountParse });
			gridMessages.ListGridColumns.Add(new UI.GridColumn("Patient\r\nPhone",100,HorizontalAlignment.Center) { SortingStrategy=UI.GridSortingStrategy.StringCompare });
			gridMessages.ListGridColumns.Add(new UI.GridColumn("Patient",150,HorizontalAlignment.Left) { SortingStrategy=UI.GridSortingStrategy.StringCompare });
			gridMessages.ListGridColumns.Add(new UI.GridColumn("Cost",32,HorizontalAlignment.Right) { SortingStrategy=UI.GridSortingStrategy.AmountParse });
			if(PrefC.HasClinicsEnabled) {
				gridMessages.ListGridColumns.Add(new UI.GridColumn("Clinic",130,HorizontalAlignment.Left) { SortingStrategy=UI.GridSortingStrategy.StringCompare });
			}
			if(checkHidden.Checked) {
				gridMessages.ListGridColumns.Add(new UI.GridColumn("Hidden",46,HorizontalAlignment.Center){SortingStrategy=UI.GridSortingStrategy.StringCompare});
			}
			foreach(SmsFromMobile smsFromMobile in _listSmsFromMobile) {
				if(!checkHidden.Checked && smsFromMobile.IsHidden) {
					continue;
				}
				UI.GridRow row=new UI.GridRow();
				row.Tag=smsFromMobile;
				if(smsFromMobile.SmsStatus==SmsFromStatus.ReceivedUnread) {
					row.Bold=true;
				}
				row.Cells.Add(smsFromMobile.DateTimeReceived.ToString());//DateTime
				row.Cells.Add(Lan.g(this,"Received"));//Type
				row.Cells.Add(SmsFromMobiles.GetSmsFromStatusDescript(smsFromMobile.SmsStatus));//Status
				row.Cells.Add(smsFromMobile.MatchCount.ToString());//#Phone Matches
				row.Cells.Add(smsFromMobile.MobilePhoneNumber);//Patient Phone
				row.Cells.Add(smsFromMobile.PatNum==0 ? Lan.g(this,"Unassigned") : GetPatientName(smsFromMobile.PatNum));//Patient
				row.Cells.Add("0.00");//Cost
				if(PrefC.HasClinicsEnabled) {
					if(smsFromMobile.ClinicNum==0) {
						row.Cells.Add(PrefC.GetString(PrefName.PracticeTitle)+" ("+Lan.g(this,"Practice")+")");
					}
					else { 
						Clinic clinic=Clinics.GetClinic(smsFromMobile.ClinicNum);
						row.Cells.Add(clinic.Abbr);//Clinic
					}
				}
				if(checkHidden.Checked) {
					row.Cells.Add(smsFromMobile.IsHidden?"X":"");//Hidden
				}
				if(selectedPatNum!=0 && smsFromMobile.PatNum==selectedPatNum) {
					row.ColorBackG=_colorSelect;
				}
				gridMessages.ListGridRows.Add(row);
			}
			if(checkSent.Checked) {
				foreach(SmsToMobile smsToMobile in _listSmsToMobile) {
					if(!checkHidden.Checked && smsToMobile.IsHidden) {
						continue;
					}
					UI.GridRow row=new UI.GridRow();
					row.Tag=smsToMobile;
					row.Cells.Add(smsToMobile.DateTimeSent.ToString());//DateTime
					row.Cells.Add(Lan.g(this,"Sent"));//Type
					string smsStatus=smsToMobile.SmsStatus.ToString(); //Default to the actual status.
					switch(smsToMobile.SmsStatus) {
						case SmsDeliveryStatus.DeliveryConf:
						case SmsDeliveryStatus.DeliveryUnconf:
							//Treated the same as far as the user is concerned.
							smsStatus=Lan.g(this,"Delivered");
							break;
						case SmsDeliveryStatus.FailWithCharge:
						case SmsDeliveryStatus.FailNoCharge:
							//Treated the same as far as the user is concerned.
							smsStatus=Lan.g(this,"Failed");
							break;
					}
					row.Cells.Add(smsStatus);//Status
					row.Cells.Add("-");//#Phone Matches, not applicable to outbound messages.
					row.Cells.Add(smsToMobile.MobilePhoneNumber);//Patient Phone
					row.Cells.Add(smsToMobile.PatNum==0 ? "" : GetPatientName(smsToMobile.PatNum));//Patient
					row.Cells.Add(smsToMobile.MsgChargeUSD.ToString("f"));//Cost
					if(PrefC.HasClinicsEnabled) {
						if(smsToMobile.ClinicNum==0) {
							row.Cells.Add(PrefC.GetString(PrefName.PracticeTitle)+" ("+Lan.g(this,"Practice")+")");
						}
						else { 
							Clinic clinic=Clinics.GetClinic(smsToMobile.ClinicNum);
							row.Cells.Add(clinic.Abbr);//Clinic
						}
					}
					if(checkHidden.Checked) {
						row.Cells.Add(smsToMobile.IsHidden?"X":"");//Hidden
					}
					if(selectedPatNum!=0 && smsToMobile.PatNum==selectedPatNum) {
						row.ColorBackG=_colorSelect;
					}
					gridMessages.ListGridRows.Add(row);
				}
			}
			gridMessages.EndUpdate();
			gridMessages.SortForced(sortByColIdx,isSortAsc);
			//Check new grid rows against previous selection and re-select.			
			for(int i=0;i<gridMessages.ListGridRows.Count;i++) { 
				if(gridMessages.ListGridRows[i].Tag is SmsFromMobile && selectedSmsFromMobile!=null
					&& ((SmsFromMobile)gridMessages.ListGridRows[i].Tag).SmsFromMobileNum==selectedSmsFromMobile.SmsFromMobileNum) 
				{
					gridMessages.SetSelected(i,true);
					break;
				}
				if(gridMessages.ListGridRows[i].Tag is SmsToMobile && selectedSmsToMobile!=null
					&& ((SmsToMobile)gridMessages.ListGridRows[i].Tag).SmsToMobileNum==selectedSmsToMobile.SmsToMobileNum) 
				{
					gridMessages.SetSelected(i,true);
					break;
				}
			}
			FillGridMessageThread();
		}

		string GetDeliverStatus(SmsDeliveryStatus smsStatus) {
			switch(smsStatus) {
				case SmsDeliveryStatus.DeliveryConf:
				case SmsDeliveryStatus.DeliveryUnconf:
					//Treated the same as far as the user is concerned.
					return Lan.g(this,"Delivered");
				case SmsDeliveryStatus.FailWithCharge:
				case SmsDeliveryStatus.FailNoCharge:
					//Treated the same as far as the user is concerned.
					return Lan.g(this,"Failed");
			}
			//Default to the actual status.
			return smsStatus.ToString();
		}

		private void FillGridTextMessagesGroupedYes(int sortByColIdx,bool isSortAsc,long selectedPatNum,TextMessageGrouped selectedSmsGroup) {
			gridMessages.Title=Lan.g(this,"Text Messages - Grouped by "+(radioGroupByPatient.Checked ? "patient" : "phone number"));
			gridMessages.BeginUpdate();
			gridMessages.ListGridRows.Clear();
			gridMessages.ListGridColumns.Clear();
			gridMessages.ListGridColumns.Add(new UI.GridColumn("DateTime",140,HorizontalAlignment.Left) { SortingStrategy=UI.GridSortingStrategy.DateParse });
			gridMessages.ListGridColumns.Add(new UI.GridColumn("Status",100,HorizontalAlignment.Left) { SortingStrategy=UI.GridSortingStrategy.StringCompare });
			_columnStatusIdx=gridMessages.ListGridColumns.Count-1;
			gridMessages.ListGridColumns.Add(new UI.GridColumn("Patient\r\nPhone",100,HorizontalAlignment.Center) { SortingStrategy=UI.GridSortingStrategy.StringCompare });
			gridMessages.ListGridColumns.Add(new UI.GridColumn("Patient",150,HorizontalAlignment.Left) { SortingStrategy=UI.GridSortingStrategy.StringCompare });
			if(PrefC.HasClinicsEnabled) {
				gridMessages.ListGridColumns.Add(new UI.GridColumn("Clinic",130,HorizontalAlignment.Left) { SortingStrategy=UI.GridSortingStrategy.StringCompare });
			}
			gridMessages.ListGridColumns.Add(new UI.GridColumn("Latest Message",150,HorizontalAlignment.Left) { SortingStrategy=UI.GridSortingStrategy.StringCompare });			
			List<TextMessageGrouped> groupAll=GetMessageGroups();
			foreach(TextMessageGrouped sms in groupAll) {				
				UI.GridRow row=new UI.GridRow();
				row.Tag=sms;
				if(sms.HasUnread) {
					row.Bold=true;
				}
				row.Cells.Add(sms.DateTimeMostRecent.ToString());
				row.Cells.Add(sms.Status);
				row.Cells.Add(sms.PatPhone);//Patient Phone
				row.Cells.Add(sms.PatName);
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(sms.ClinicAbbr);
				}
				row.Cells.Add(sms.TextMsg);
				gridMessages.ListGridRows.Add(row);
			}
			gridMessages.EndUpdate();
			gridMessages.SortForced(sortByColIdx,isSortAsc);
			//Check new grid rows against previous selection and re-select.			
			for(int i=0;i<gridMessages.ListGridRows.Count;i++) {
				if(
					gridMessages.ListGridRows[i].Tag is TextMessageGrouped && 
					selectedSmsGroup!=null &&
					((TextMessageGrouped)gridMessages.ListGridRows[i].Tag).PatPhone==selectedSmsGroup.PatPhone &&
					((TextMessageGrouped)gridMessages.ListGridRows[i].Tag).PatNum==selectedSmsGroup.PatNum)
				{
					gridMessages.SetSelected(i,true);
					break;
				}
			}
			FillGridMessageThread();
		}

		///<summary>Gets messages groups to display in the grid for when group by 'Patient' or 'Phone Number' is selected.</summary>
		private List<TextMessageGrouped> GetMessageGroups() {
			List<TextMessageGrouped> groupToMobile=new List<TextMessageGrouped>();
			List<TextMessageGrouped> groupFromMobile=_listSmsFromMobile
				.Where(x => (x.IsHidden && checkHidden.Checked) || !x.IsHidden)
				.OrderByDescending(x => x.DateTimeReceived)
				//Assigning all groups a PatNum of 0 will effectively group them by phone number only
				.GroupBy(x => new { x.MobilePhoneNumber,PatNum=(radioGroupByPatient.Checked ? x.PatNum : 0) })
				.Select(x => new TextMessageGrouped() {
					DateTimeMostRecent=x.First().DateTimeReceived,
					PatPhone=x.First().MobilePhoneNumber,
					PatNum=x.First().PatNum,
					ClinicNum=x.First().ClinicNum,
					ClinicAbbr=PrefC.HasClinicsEnabled ? (x.First().ClinicNum==0 ? PrefC.GetString(PrefName.PracticeTitle)+
						" ("+Lan.g(this,"Practice")+")" : Clinics.GetClinic(x.First().ClinicNum).Abbr) : "",
					PatName=x.First().PatNum==0 ? Lan.g(this,"Unassigned") : GetPatientName(x.First().PatNum),
					TextMsg=x.First().MsgText,
					HasUnread=x.Any(y => y.SmsStatus==SmsFromStatus.ReceivedUnread),
					Status=Lan.g(this,"Rcv")+" - "+SmsFromMobiles.GetSmsFromStatusDescript(x.First().SmsStatus),
					ListToMobile=new List<SmsToMobile>(),
					ListFromMobile=x.ToList(),
				}).ToList();
			if(checkSent.Checked) {
				groupToMobile=_listSmsToMobile
					.Where(x => (x.IsHidden && checkHidden.Checked) || !x.IsHidden)
					.OrderByDescending(x => x.DateTimeSent)
					//Assigning all groups a PatNum of 0 will effectively group them by phone number only
					.GroupBy(x => new { x.MobilePhoneNumber,PatNum = (radioGroupByPatient.Checked ? x.PatNum : 0) })
					.Select(x => new TextMessageGrouped() {
						DateTimeMostRecent=x.First().DateTimeSent,
						PatPhone=x.First().MobilePhoneNumber,
						PatNum=x.First().PatNum,
						ClinicNum=x.First().ClinicNum,
						ClinicAbbr=PrefC.HasClinicsEnabled ? (x.First().ClinicNum==0 ? PrefC.GetString(PrefName.PracticeTitle)+" ("+Lan.g(this,"Practice")+")"
							: Clinics.GetClinic(x.First().ClinicNum).Abbr) : "",
						PatName=x.First().PatNum==0 ? Lan.g(this,"Unassigned") : GetPatientName(x.First().PatNum),
						TextMsg=x.First().MsgText,
						HasUnread=false,
						Status=Lan.g(this,"Sent")+" - "+GetDeliverStatus(x.First().SmsStatus),
						ListFromMobile=new List<SmsFromMobile>(),
						ListToMobile=x.ToList(),
					}).ToList();
			}
			List<TextMessageGrouped> groupAll=groupFromMobile.Union(groupToMobile)
				.OrderByDescending(x => x.DateTimeMostRecent)
				//Assigning all groups a PatNum of 0 will effectively group them by phone number only
				.GroupBy(x => new { x.PatPhone,PatNum=(radioGroupByPatient.Checked ? x.PatNum : 0) })
				.Select(x => {
					//First entry from the union.
					TextMessageGrouped group=x.First();
					//Patch the list back together
					group.HasUnread=x.Any(y => y.HasUnread);
					group.ListToMobile=x.SelectMany(y => y.ListToMobile).ToList();
					group.ListFromMobile=x.SelectMany(y => y.ListFromMobile).ToList();
					return group;
				}).ToList();
			return groupAll;
		}		

		private void FillGridMessageThread() {
			if(_selectedPatNum==0) {
				Text=Lan.g(this,"Text Messaging");
			}
			else {
				Text=Lan.g(this,"Text Messaging - Patient:")+" "+GetPatientName(_selectedPatNum);
			}
			labelPatientsForPhone.Visible=false;
			if(_selectedPatNum==0 && _selectedSmsGroup==null) { //A message with no patNum was selected and is not a group, or nothing is selected at all.
				if(_selectedSmsToMobile!=null) { //SmsToMobile is selected.
					smsThreadView.ListSmsThreadMessages=new List<SmsThreadMessage>() { new SmsThreadMessage(_selectedSmsToMobile.DateTimeSent,_selectedSmsToMobile.MsgText,true,true,true) };
				}
				else if(_selectedSmsFromMobile!=null) { //SmsFromMobile is selected.
					smsThreadView.ListSmsThreadMessages=new List<SmsThreadMessage>() { new SmsThreadMessage(_selectedSmsFromMobile.DateTimeReceived,_selectedSmsFromMobile.MsgText,true,true,true) };
				}
				else { //Nothing selected at all, clear the message thread.
					smsThreadView.ListSmsThreadMessages=null;
				}
				SetReplyHelper();
				return;
			}
			List<SmsThreadMessage> listSmsThreadMessages=new List<SmsThreadMessage>();
			long patNumForDb;
			string numberForDb;
			if(_isGroupedByPhone) {//Grouped by phone. Get all messages from the database for the given phone number. Don't specify patNum.
				patNumForDb=-1;
				numberForDb=_selectedMobileNumber;
			}
			else if(_selectedPatNum==0 && _isGrouped) {//Grouped by Patient and the patient's patnum is 0. Find messages with the patnum/number combo.
				patNumForDb=_selectedPatNum;
				numberForDb=_selectedMobileNumber;
			}
			else {//Normal patnum. Find all messages for the patnum regardless of number.
				patNumForDb=_selectedPatNum;
				numberForDb="";
			}
			List<SmsFromMobile> listSmsFromMobile=SmsFromMobiles.GetMessages(DateTime.MinValue,DateTime.MinValue,GetListSelectedClinicNums(),
				patNumForDb,true,numberForDb);
			foreach(SmsFromMobile smsFromMobile in listSmsFromMobile) {
				bool isHighlighted=false;
				if(_selectedSmsFromMobile!=null && _selectedSmsFromMobile.SmsFromMobileNum==smsFromMobile.SmsFromMobileNum) {
					isHighlighted=true;
				}
				listSmsThreadMessages.Add(new SmsThreadMessage(smsFromMobile.DateTimeReceived,smsFromMobile.MsgText,true,false,isHighlighted));
			}
			List<SmsToMobile> listSmsToMobile=SmsToMobiles.GetMessages(DateTime.MinValue,DateTime.MinValue,GetListSelectedClinicNums(),patNumForDb,
				numberForDb);
			foreach(SmsToMobile smsToMobile in listSmsToMobile) {
				bool isHighlighted=false;
				if(_selectedSmsToMobile!=null && _selectedSmsToMobile.SmsToMobileNum==smsToMobile.SmsToMobileNum) {
					isHighlighted=true;
				}
				bool isImportant=false;
				if(smsToMobile.SmsStatus==SmsDeliveryStatus.FailNoCharge ||smsToMobile.SmsStatus==SmsDeliveryStatus.FailWithCharge) {
					isImportant=true;
				}
				listSmsThreadMessages.Add(new SmsThreadMessage(smsToMobile.DateTimeSent,smsToMobile.MsgText,false,isImportant,isHighlighted));
			}
			listSmsThreadMessages.Sort(SmsThreadMessage.CompareMessages);
			smsThreadView.ListSmsThreadMessages=listSmsThreadMessages;
			List<long> listPatNumsInMessages=listSmsFromMobile.Select(x => x.PatNum).Union(listSmsToMobile.Select(x => x.PatNum)).Distinct().ToList();
			string patNames="";
			labelPatientsForPhone.Visible=false;
			if(listPatNumsInMessages.Count > 1) {//Only display the patient names if there are more than 1.
				patNames=string.Join(", ",listPatNumsInMessages.Select(x => GetPatientName(x)).Distinct());
				labelPatientsForPhone.Visible=true;
			}
			labelPatientsForPhone.Text=patNames;
			SetReplyHelper();
		}
		#endregion

		private void SetReplyHelper() {
			if(!_hasSelectedMessage) {
				butSend.Enabled=false;
				textReply.Enabled=false;
				textReply.Text="";
				return;
			}
			butSend.Enabled=true;
			textReply.Enabled=true;
		}
		
		#region Control Event Handlers
		private void butPatCurrent_Click(object sender,EventArgs e) {
			_patNumCur=(FormOpenDental.CurPatNum==0 ? -1 : FormOpenDental.CurPatNum);
			if(_patNumCur==-1) {
				textPatient.Text="";
			}
			else {
				textPatient.Text=Patients.GetLim(_patNumCur).GetNameLF();
			}
		}

		private void butPatFind_Click(object sender,EventArgs e) {
			using FormPatientSelect FormP=new FormPatientSelect();
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK) {
				return;
			}
			_patNumCur=FormP.SelectedPatNum;
			textPatient.Text=Patients.GetLim(_patNumCur).GetNameLF();
		}

		private void butPatAll_Click(object sender,EventArgs e) {
			_patNumCur=-1;
			textPatient.Text="";
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			FillGridTextMessages();
			Cursor=Cursors.Default;
		}

		///<summary>Since the grid is set to SelectionMode of "One", there will always be exactly 1 selection when this event occurs.
		///Initially the grid does not have any selections, but this event will not be called the first time until the first row is selected.
		///Additionally, there is no way for the user to deselect a row when SelectionMode is set to "One", the user can only change the selection.</summary>
		private void gridMessages_SelectionCommitted(object sender,EventArgs e) {
			List<SmsFromMobile> listMarkReceivedRead=new List<SmsFromMobile>();
			//If the clicked/selected message was a ReceivedUnread message, then mark the message ReceivedRead in the db as well as the grid.
			if(_selectedSmsFromMobile!=null && _selectedSmsFromMobile.SmsStatus==SmsFromStatus.ReceivedUnread) {
				listMarkReceivedRead.Add(_selectedSmsFromMobile);
			}
			if(_selectedSmsGroup!=null) {
				listMarkReceivedRead.AddRange(_selectedSmsGroup.ListFromMobile.FindAll(x => x.SmsStatus==SmsFromStatus.ReceivedUnread));
			}
			if(listMarkReceivedRead.Count>0) {
				//Notifier is intentionally invoked before updating the sms row. This will allow local count to be modified without the risk of a race condition.			
				_smsNotifier?.Invoke(-listMarkReceivedRead.Count);
				//Messages were marked as 'Read' so update the db and update the grid in place.
				foreach(SmsFromMobile fromMobile in listMarkReceivedRead) {
					SmsFromMobile oldSmsFromMobile=fromMobile.Copy();
					fromMobile.SmsStatus=SmsFromStatus.ReceivedRead;
					SmsFromMobiles.Update(fromMobile,oldSmsFromMobile);
				}
				//Fix the rows in place. Forcing an entire refresh would cause sorting of grid not to persist.
				gridMessages.ListGridRows[gridMessages.SelectedIndices[0]].Bold=false;
				var tag=gridMessages.ListGridRows[gridMessages.SelectedIndices[0]].Tag;
				if(tag is TextMessageGrouped) {
					TextMessageGrouped smsGroup=(TextMessageGrouped)tag;
					smsGroup.HasUnread=false;
					//Latest entry wins for this column value.
					var latestFromMobile=smsGroup.ListFromMobile.OrderByDescending(x => x.DateTimeReceived).FirstOrDefault()??new SmsFromMobile() { DateTimeReceived=DateTime.MinValue };
					var latestToMobile=smsGroup.ListToMobile.OrderByDescending(x => x.DateTimeSent).FirstOrDefault()??new SmsToMobile() {DateTimeSent=DateTime.MinValue };
					gridMessages.ListGridRows[gridMessages.SelectedIndices[0]].Cells[_columnStatusIdx].Text=
						latestFromMobile.DateTimeReceived>latestToMobile.DateTimeSent ? 
							Lan.g(this,"Rcv")+" - "+SmsFromMobiles.GetSmsFromStatusDescript(SmsFromStatus.ReceivedRead) : 
							Lan.g(this,"Sent")+" - "+GetDeliverStatus(latestToMobile.SmsStatus);
				}
				else if(tag is SmsFromMobile) {
					SmsFromMobile smsFrom=(SmsFromMobile)tag;
					smsFrom.SmsStatus=SmsFromStatus.ReceivedRead;
					gridMessages.ListGridRows[gridMessages.SelectedIndices[0]].Cells[_columnStatusIdx].Text=SmsFromMobiles.GetSmsFromStatusDescript(SmsFromStatus.ReceivedRead);
				}			
			}
			//Update highlighted rows.
			long selectedPatNum=_selectedPatNum;
			gridMessages.ListGridRows.ToList().ForEach(x => {
				long patNum=0;
				if(x.Tag is TextMessageGrouped) {
					patNum=((TextMessageGrouped)x.Tag).PatNum;
				}
				else if(x.Tag is SmsFromMobile) {
					patNum=((SmsFromMobile)x.Tag).PatNum;					
				}
				else if(x.Tag is SmsToMobile) {
					patNum=((SmsToMobile)x.Tag).PatNum;
				}
				x.ColorBackG=patNum!=0 && selectedPatNum==patNum ? _colorSelect : Color.White;
			});
			gridMessages.Invalidate();
			FillGridMessageThread();
		}
		
		///<summary>Sets the given status for the selected receieved message.</summary>
		private void SetReceivedSelectedStatus(SmsFromStatus smsFromStatus) {
			if(_selectedSmsGroup!=null) {
				MsgBox.Show(this,"Please turn off Group By Patient.");
				return;
			}
			if(_selectedSmsFromMobile==null) {
				MsgBox.Show(this,"Please select a received message.");
				return;
			}
			Cursor=Cursors.WaitCursor;			
			UI.GridRow row=gridMessages.ListGridRows[gridMessages.GetSelectedIndex()];
			SmsFromMobile smsFromMobile=_selectedSmsFromMobile;
			SmsFromMobile oldSmsFromMobile=smsFromMobile.Copy();
			smsFromMobile.SmsStatus=smsFromStatus;
			//Notifier is intentionally invoked before updating the sms row. This will allow local count to be modified without the risk of a race condition.
			_smsNotifier?.Invoke(smsFromStatus==SmsFromStatus.ReceivedRead ? -1 : 1);
			SmsFromMobiles.Update(smsFromMobile,oldSmsFromMobile);
			row.Cells[_columnStatusIdx].Text=SmsFromMobiles.GetSmsFromStatusDescript(smsFromStatus);
			row.Bold=false;
			if(smsFromStatus==SmsFromStatus.ReceivedUnread) {
				row.Bold=true;
			}			
			gridMessages.Invalidate();//To show the status changes in the grid.
			Cursor=Cursors.Default;
		}

		private void menuItemChangePat_Click(object sender,EventArgs e) {
			if(_selectedSmsGroup!=null) {
				MsgBox.Show(this,"Please turn off Group By Patient.");
				return;
			}
			if(_selectedSmsFromMobile==null) {
				MsgBox.Show(this,"Please select a received message.");
				return;
			}
			using FormPatientSelect form=new FormPatientSelect();
			List<Patient> listPats=Patients.GetPatientsByPhone(_selectedSmsFromMobile.MobilePhoneNumber,				
				CultureInfo.CurrentCulture.Name.Substring(CultureInfo.CurrentCulture.Name.Length-2));//Country code of current environment.
			if(Clinics.ClinicNum!=0) {
				List<long> listClinicNums=GetListSelectedClinicNums().Union(new List<long>() { _selectedSmsFromMobile.ClinicNum }).ToList();
				listPats=listPats.Where(x => listClinicNums.Contains(x.ClinicNum)).ToList();
			}
			form.ExplicitPatNums=listPats.Select(x=> x.PatNum).ToList();
			if(form.ShowDialog()!=DialogResult.OK) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			SmsFromMobile smsFromMobile=_selectedSmsFromMobile;
			SmsFromMobile oldSmsFromMobile=smsFromMobile.Copy();
			smsFromMobile.PatNum=form.SelectedPatNum;
			SmsFromMobiles.Update(smsFromMobile,oldSmsFromMobile);
			//Upsert the Commlog.
			if(smsFromMobile.CommlogNum==0) {
				//When a new sms comes in on the server, a corresponding commlog is inserted, unless the sms could not be matched to a patient by phone #.
				//We need to insert a new commlog when the patient is selected, for the case when the message has not been asssigned to a patient yet.
				//This way we can ensure that all sms with a patient attached have a corresponding commlog.
				Commlog commlog=new Commlog();
				commlog.CommDateTime=smsFromMobile.DateTimeReceived;
				commlog.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.TEXT);
				commlog.Mode_=CommItemMode.Text;
				commlog.Note=smsFromMobile.MsgText;
				commlog.PatNum=smsFromMobile.PatNum;
				commlog.SentOrReceived=CommSentOrReceived.Received;
				Commlogs.Insert(commlog);
			}
			else {
				Commlog commlog=Commlogs.GetOne(smsFromMobile.CommlogNum);
				Commlog oldCommlog=commlog.Copy();
				commlog.PatNum=form.SelectedPatNum;
				Commlogs.Update(commlog,oldCommlog);
			}			
			FillGridTextMessages();//Refresh grid to show changed patient.
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Text message was moved successfully");
		}

		private void menuItemMarkUnread_Click(object sender,EventArgs e) {
			SetReceivedSelectedStatus(SmsFromStatus.ReceivedUnread);
		}

		private void menuItemMarkRead_Click(object sender,EventArgs e) {
			SetReceivedSelectedStatus(SmsFromStatus.ReceivedRead);
		}

		///<summary>Set isHide to true to hide the selected message.  Set IsHide to false to unhide the selected message.</summary>
		private void HideOrUnhideMessages(bool isHide) {
			if(!_hasSelectedMessage) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			if(_selectedSmsFromMobile!=null) {
				SmsFromMobile oldSmsFromMobile=_selectedSmsFromMobile.Copy();
				_selectedSmsFromMobile.IsHidden=isHide;
				SmsFromMobiles.Update(_selectedSmsFromMobile,oldSmsFromMobile);
			}
			else if(_selectedSmsToMobile!=null) {
				SmsToMobile oldSmsToMobile=_selectedSmsToMobile.Copy();
				_selectedSmsToMobile.IsHidden=isHide;
				SmsToMobiles.Update(_selectedSmsToMobile,oldSmsToMobile);
			}
			Cursor=Cursors.Default;
			FillGridTextMessages();
		}

		private void menuItemHide_Click(object sender,EventArgs e) {
			if(!_hasSelectedMessage) {
				MsgBox.Show(this,"Please select a message before attempting to hide.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Hide selected message?")) {
				return;
			}
			HideOrUnhideMessages(true);
		}

		private void menuItemUnhide_Click(object sender,EventArgs e) {
			if(!_hasSelectedMessage) {
				MsgBox.Show(this,"Please select a message before attempting to unhide.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Unhide selected message?")) {
				return;
			}
			HideOrUnhideMessages(false);
		}

		private void menuItemBlockNumber_Click(object sender,EventArgs e) {
			if(!_hasSelectedMessage) {
				MsgBox.Show(this,"Please select a received or sent message to block.");
				return;
			}
			string numberToBlock=_selectedMobileNumber;
			string question=Lan.g(this,"Block incoming texts from")+" "+numberToBlock+"? "+Lan.g(this,"This cannot be undone.");
			if(_selectedPatNum!=0) {
				Patient pat=Patients.GetPat(_selectedPatNum);
				if(pat!=null) {
					question+="\r\n"+Lan.g(this,"This phone number is attached to patient")+" "+pat.GetNameFLnoPref()+".";
				}
			}
			if(MessageBox.Show(question,"",MessageBoxButtons.YesNo)==DialogResult.No) {
				return;
			}
			SmsBlockPhones.Insert(new SmsBlockPhone { BlockWirelessNumber=numberToBlock });
			DataValid.SetInvalid(InvalidType.SmsBlockPhones);
		}
				
		private void menuItemSelectPatient_Click(object sender,EventArgs e) {
			if(!_hasSelectedMessage) {
				MsgBox.Show(this,"Please select a message first.");
				return;
			}			
			if(_selectedPatNum==0 || GetPatientName(_selectedPatNum)==Lan.g(this,"Not found")) {//If the patNum is 0 or patient could not be found.
				MsgBox.Show(this,"Please select a message with a valid patient attached.");
				return;
			}
			FormOpenDental.S_Contr_PatientSelected(Patients.GetPat(_selectedPatNum),true);
		}

		private void butSend_Click(object sender,EventArgs e) {
			if(!_hasSelectedMessage) {
				MsgBox.Show(this,"Please select a message thread to reply to.");
				return;
			}
			if(textReply.Text=="") {
				MsgBox.Show(this,"Please input reply text first.");
				return;
			}
			long clinicNum=0;
			if(!PrefC.HasClinicsEnabled) {
				clinicNum=0;
			}
			else if(_selectedSmsGroup!=null) {
				clinicNum=_selectedSmsGroup.ClinicNum;//can be 0
			}
			else if(_selectedSmsFromMobile!=null) {
				clinicNum=_selectedSmsFromMobile.ClinicNum;//can be 0
			}
			else if(_selectedSmsToMobile!=null) {
				clinicNum=_selectedSmsToMobile.ClinicNum;//can be 0
			}
			if(string.IsNullOrEmpty(_selectedMobileNumber)) {
				//should never happen.
				MsgBox.Show(this,"Selected message does not have a valid phone number to send to.");
				return;
			}
			if(PrefC.HasClinicsEnabled && clinicNum==0) {
				clinicNum=PrefC.GetLong(PrefName.TextingDefaultClinicNum);
				if(clinicNum==0) {
					MsgBox.Show(this,"No default clinic setup for texting.");
					return;
				}
			}
			//Verify that the highlighted blue rows and grey selected row match before allowing the user to send the message.
			//This was happening for the user due to the way ODGrid.CellClick functions.  This issue should now be fixed, but this is a catch-all
			//to ensure that it is impossible for the user to send to the wrong patient due to invalid selections.
			bool isInvalidSelection=false;
			for(int i=0;i<gridMessages.ListGridRows.Count;i++) {
				UI.GridRow row=gridMessages.ListGridRows[i];
				if(row.ColorBackG!=_colorSelect) { //Verify that the row the user appears to have selected is actually the selected row.
					continue;
				}
				if(row.Tag is TextMessageGrouped) {
					TextMessageGrouped smsGroup=(TextMessageGrouped)row.Tag;
					if(smsGroup.PatNum!=_selectedPatNum) {
						gridMessages.SetSelected(i,true);//Change the selection to the first highlighted row.
						isInvalidSelection=true;
						break;
					}
				}
				else if(row.Tag is SmsFromMobile) {
					SmsFromMobile smsFrom=(SmsFromMobile)row.Tag;
					if(smsFrom.PatNum!=_selectedPatNum) {
						gridMessages.SetSelected(i,true);//Change the selection to the first highlighted row.
						isInvalidSelection=true;
						break;
					}
				}
				else if(row.Tag is SmsToMobile) {
					SmsToMobile smsTo=(SmsToMobile)row.Tag;
					if(smsTo.PatNum!=_selectedPatNum) {
						gridMessages.SetSelected(i,true);//Change the selection to the first highlighted row.
						isInvalidSelection=true;
						break;
					}
				}
			}
			if(isInvalidSelection) {
				FillGridTextMessages(true);//This ensures that the selected row and highlighted rows are for the same patient as displayed in the grid.
				MsgBox.Show(this,"Message selection was not showing correctly in the grid.  Selections have been refreshed.  "
					+"Please review selected message thread before sending.");
				return;
			}
			try {
				if(Plugins.HookMethod(this,"FormSmsTextMessaging.butReply_Click_sendSmsSingle",_selectedPatNum,_selectedMobileNumber,textReply.Text,YN.Yes)) {
					goto HookSkipSmsCall;
				}
				SmsToMobiles.SendSmsSingle(_selectedPatNum,_selectedMobileNumber,textReply.Text,clinicNum,SmsMessageSource.DirectSms,user: Security.CurUser);
			}
			catch(Exception ex) {
				if(!FormEServicesSetup.ProcessSendSmsException(ex)) {
					MsgBox.Show(this,ex.Message);
				}
				return;
			}
			HookSkipSmsCall: { }
			textReply.Text="";
			FillGridMessageThread();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
		#endregion

		#region Helper class
		private class TextMessageGrouped {
			public DateTime DateTimeMostRecent;
			public string PatPhone;
			public string PatName;
			public long PatNum;
			public string ClinicAbbr;
			public long ClinicNum;
			public string TextMsg;
			public bool HasUnread=false;
			public string Status;
			public List<SmsFromMobile> ListFromMobile=new List<SmsFromMobile>();
			public List<SmsToMobile> ListToMobile=new List<SmsToMobile>();
		}
		#endregion
		
		/*
		private void InsertDemoData() {
			if(ODBuild.IsDebug()) {
				bool insertInboud=false;
				bool insertOutbound=false;
				if(!insertInboud&&!insertOutbound) {
					return;
				}
				Random rand=new Random();
				var clinics=Clinics.GetDeepCopy();
				clinics.Add(Clinics.GetPracticeAsClinicZero());
				var vlns=SmsPhones.GetAll();
				var patients=Patients.GetAllPatients()
					.FindAll(x => !string.IsNullOrEmpty(x.HmPhone))
					.GroupBy(x => x.ClinicNum)
					.ToDictionary(x => x.Key,x => x.ToList());
				Action addMissingVLNs=new Action(() => {
					var missingVlns=patients.Keys
						.Select(x => x)
						.Where(x => !vlns.Any(y => y.ClinicNum==x)).ToList();
					foreach(var missingVln in missingVlns) {
						SmsPhones.Insert(new SmsPhone() {
							ClinicNum=missingVln,
							CountryCode="US",
							DateTimeActive=DateTime.Now.Subtract(TimeSpan.FromDays(1)),
							PhoneNumber=rand.Next(0,999999999).ToString("D10"),
						});
					}
					vlns=SmsPhones.GetAll();
				});
				if(insertInboud) {
					List<SmsFromMobile> listInbound=new List<SmsFromMobile>();
					int count=rand.Next(100,200);
					addMissingVLNs();
					for(int i = 0;i<count;i++) {
						string unique=rand.Next(0,1000000).ToString("D7");
						SmsPhone vln=vlns[rand.Next(0,vlns.Count)];
						long clinicNum=vln.ClinicNum;
						if(!patients.ContainsKey(clinicNum)) {
							continue;
						}
						var patsForClinic=patients[clinicNum];
						Patient pat=patsForClinic[rand.Next(0,patsForClinic.Count)];
						string patPhone=pat.HmPhone;
						listInbound.Add(new SmsFromMobile() {
							ClinicNum=clinicNum,
							DateTimeReceived=DateTime.Now.Subtract(TimeSpan.FromMinutes(rand.Next(5,2000))),
							GuidMessage="TEST"+unique,
							MobilePhoneNumber=patPhone,
							MsgPart=1,
							MsgTotal=1,
							MsgRefID="x",
							MsgText="msg - "+unique,
							SmsPhoneNumber=vln.PhoneNumber,
							SmsStatus=SmsFromStatus.ReceivedUnread,
							PatNum=0, //Leave unassigned, we will find the match below
						});
					}
					SmsFromMobiles.ProcessInboundSms(listInbound);
				}
				if(insertOutbound) {
					List<SmsToMobile> listOutbound=new List<SmsToMobile>();
					int count=rand.Next(10,100);
					addMissingVLNs();
					for(int i = 0;i<count;i++) {
						string unique=rand.Next(0,1000000).ToString("D7");
						SmsPhone vln=vlns[rand.Next(0,vlns.Count)];
						long clinicNum=vln.ClinicNum;
						if(!patients.ContainsKey(clinicNum)) {
							continue;
						}
						var patsForClinic=patients[clinicNum];
						Patient pat=patsForClinic[rand.Next(0,patsForClinic.Count)];
						string patPhone=pat.HmPhone;
						DateTime dtSent=DateTime.Now.Subtract(TimeSpan.FromMinutes(rand.Next(5,2000)));
						listOutbound.Add(new SmsToMobile() {
							ClinicNum=clinicNum,
							DateTimeSent=dtSent,
							DateTimeTerminated=dtSent.Add(TimeSpan.FromSeconds(rand.Next(60,240))),
							GuidBatch="TEST"+unique,
							GuidMessage="TEST"+unique,
							IsTimeSensitive=true,
							MobilePhoneNumber=patPhone,
							MsgChargeUSD=.04f,
							MsgParts=1,
							MsgText="outbound - "+unique,
							MsgType=SmsMessageSource.DirectSms,
							PatNum=pat.PatNum,
							SmsPhoneNumber=vln.PhoneNumber,
							SmsStatus=SmsDeliveryStatus.DeliveryConf,
						});
					}
					listOutbound.ForEach(x => SmsToMobiles.Insert(x));
				}
			}
		}
		*/
	}
}