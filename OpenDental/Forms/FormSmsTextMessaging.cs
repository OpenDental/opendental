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
using Intuit.Ipp.Data;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormSmsTextMessaging:FormODBase {

		#region Private data; properties/fields
		///<summary>Set from FormOpenDental.  This can be null if the calling code does not wish to get dynamic unread message counts.
		///Allows FormSmsTextMessaging to update the unread SMS text message count in FormOpenDental as the user reads their messages.</summary>
		private Action<int> _actionSmsNotifier=null;
		///<summary>The selected patNum.  Can be -1 to include all.</summary>
		private long _patNum=-1;
		///<summary>The column index of the Status column within the Messages grid.
		///This is a class-wide variable to prevent bugs if we decide to change the column order of the Messages grid.</summary>
		private int _idxColumnStatus=0;
		private UserOdPref _userOdPrefGroupBy=null;
		private List<SmsFromMobile> _listSmsFromMobiles=new List<SmsFromMobile>();
		private List<SmsToMobile> _listSmsToMobiles=new List<SmsToMobile>();
		private Color _colorSelect=Color.FromArgb(224,243,255);
		private List<Patient> _listPatients=new List<Patient>();

		///<summary>True if the "Group By Patient" or "Group By Phone Number" radio buttons are checked.</summary>
		private bool IsGrouped() {
			return radioGroupByPatient.Checked || radioGroupByPhone.Checked;
		}

		///<summary>Null if gridMessage current selected row tag is not TextMessageGrouped.</summary>
		private TextMessageGrouped GetSelectedSmsGroup() {
			if(gridMessages.GetSelectedIndex()==-1) {
				return null;
			}
			object selectedTag=gridMessages.ListGridRows[gridMessages.GetSelectedIndex()].Tag;
			if(!(selectedTag is TextMessageGrouped)) {
				return null;
			}
			return (TextMessageGrouped)selectedTag;
		}

		///<summary>Null if gridMessage current selected row tag is not SmsFromMobile.</summary>
		private SmsFromMobile GetSelectedSmsFromMobile() {
			if(gridMessages.GetSelectedIndex()==-1) {
				return null;
			}
			object selectedTag=gridMessages.ListGridRows[gridMessages.GetSelectedIndex()].Tag;
			if(!(selectedTag is SmsFromMobile)) {
				return null;
			}
			return (SmsFromMobile)selectedTag;
		}

		///<summary>Null if gridMessage current selected row tag is not SmsToMobile.</summary>
		private SmsToMobile GetSelectedSmsToMobile() {
			if(gridMessages.GetSelectedIndex()==-1) {
				return null;
			}
			object selectedTag=gridMessages.ListGridRows[gridMessages.GetSelectedIndex()].Tag;
			if(!(selectedTag is SmsToMobile)) {
				return null;
			}
			return (SmsToMobile)selectedTag;
		}

		private bool HasSelectedMessage() {
			return GetSelectedSmsGroup()!=null || GetSelectedSmsFromMobile()!=null || GetSelectedSmsToMobile()!=null;
		}

		///<summary>0 if gridMessage current selected row tag is not SmsToMobile or SmsFromMobile or TextMessageGrouped.</summary>
		private long GetSelectedPatNum() {
			if(GetSelectedSmsGroup()!=null) {
				return GetSelectedSmsGroup().PatNum;
			}
			if(GetSelectedSmsFromMobile()!=null) {
				return GetSelectedSmsFromMobile().PatNum;
			}
			if(GetSelectedSmsToMobile()!=null) {
				return GetSelectedSmsToMobile().PatNum;
			}
			return 0;
		}

		///<summary>Empty if gridMessage current selected row tag is not SmsToMobile or SmsFromMobile or TextMessageGrouped.</summary>
		private string GetSelectedMobileNumber() {
			if(GetSelectedSmsGroup()!=null) {
				return GetSelectedSmsGroup().PatPhone;
			}
			if(GetSelectedSmsFromMobile()!=null) {
				return GetSelectedSmsFromMobile().MobilePhoneNumber;
			}
			if(GetSelectedSmsToMobile()!=null) {
				return GetSelectedSmsToMobile().MobilePhoneNumber;
			}
			return "";
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

		///<summary>Passively load _listPatients from db if entry is not already found.</summary>
		private string GetPatientName(long patNum) {
			AddPatientNames(new List<long>() { patNum });
			Patient patient=_listPatients.Find(x=>x.PatNum==patNum);
			if(patient is null){
				return Lan.g(this,"Not found");
			}
			return patient.GetNameLF();
		}

		///<summary>Safe to call this for any number of PatNums. Will only go to db if given PatNum(s) not already found in _dictPatNames.</summary>
		private void AddPatientNames(List<long> listPatNums) {
			List<long> listPatNumsNew=listPatNums.FindAll(x=>x!=0 && !_listPatients.Exists(y=>y.PatNum==x));
			if(listPatNumsNew.Count()>0) {
				List<Patient> listPatientsNew=Patients.GetLimForPats(listPatNumsNew);
				_listPatients.AddRange(listPatientsNew);
			}
		}
		#endregion

		#region Init
		public FormSmsTextMessaging(bool isSent,bool isReceived,Action<int> actionSmsNotifier) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			checkSent.Checked=isSent;
			checkRead.Checked=isReceived;
			_actionSmsNotifier=actionSmsNotifier;
			_userOdPrefGroupBy=UserOdPrefs.GetFirstOrNewByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.SmsGroupBy);
			if(_userOdPrefGroupBy.ValueString=="1") {
				radioGroupByPatient.Checked=true;
			}
			else if(_userOdPrefGroupBy.ValueString=="2") {
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
					_userOdPrefGroupBy.ValueString="0";
				}
				else if(radioGroupByPatient.Checked) {
					_userOdPrefGroupBy.ValueString="1";
				}
				else if(radioGroupByPhone.Checked) {
					_userOdPrefGroupBy.ValueString="2";
				}
				UserOdPrefs.Upsert(_userOdPrefGroupBy);
				DataValid.SetInvalid(InvalidType.UserOdPrefs);
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
			menuItemChangePat.Visible=!IsGrouped();
			menuItemMarkUnread.Visible=!IsGrouped();
			menuItemMarkRead.Visible=!IsGrouped();
			menuItemHide.Visible=!IsGrouped();
			menuItemUnhide.Visible=!IsGrouped();
			menuItemBlockNumber.Visible=!IsGrouped();
			if(PrefC.HasClinicsEnabled && comboClinics.ListSelectedClinicNums.Count==0) {
				gridMessages.BeginUpdate();
				gridMessages.ListGridRows.Clear();
				gridMessages.EndUpdate();
				return;
			}
			//Hold these. We will be clearing them below and they will need to be restored.
			int idxColSortBy=gridMessages.SortedByColumnIdx;
			bool isSortAsc=gridMessages.SortedIsAscending;
			if(idxColSortBy==-1 || !retainSort) {
				idxColSortBy=0;
				isSortAsc=false;
			}
			if(!isRedrawOnly) {
				List<SmsFromStatus> listSmsFromStatuses=new List<SmsFromStatus>();
				listSmsFromStatuses.Add(SmsFromStatus.ReceivedUnread);
				if(checkRead.Checked){
					listSmsFromStatuses.Add(SmsFromStatus.ReceivedRead);
				}
				_listSmsFromMobiles=SmsFromMobiles.GetMessages(PIn.Date(textDateFrom.Text),PIn.Date(textDateTo.Text),GetListSelectedClinicNums(),_patNum,false,"",listSmsFromStatuses);
				if(checkSent.Checked) {
					_listSmsToMobiles=SmsToMobiles.GetMessages(PIn.Date(textDateFrom.Text),PIn.Date(textDateTo.Text),GetListSelectedClinicNums(),_patNum,"");
				}
				AddPatientNames(_listSmsFromMobiles.GroupBy(x => x.PatNum).Select(x => x.Key)
					.Union(_listSmsToMobiles.GroupBy(x => x.PatNum).Select(x => x.Key)).ToList());
			}
			if(IsGrouped()) {
				FillGridTextMessagesGroupedYes(idxColSortBy,isSortAsc,GetSelectedPatNum(),GetSelectedSmsGroup());
				return;
			}
			FillGridTextMessagesGroupedNo(idxColSortBy,isSortAsc,GetSelectedPatNum(),GetSelectedSmsToMobile(),GetSelectedSmsFromMobile());
		}

		private void FillGridTextMessagesGroupedNo(int idxColSortBy,bool isSortAsc,long patNumSelected,SmsToMobile smsToMobileSelected,SmsFromMobile smsFromMobileSelected) {
			gridMessages.Title=Lan.g(this,"Text Messages - Right click for options - Unread messages always shown");
			gridMessages.BeginUpdate();
			gridMessages.ListGridRows.Clear();
			gridMessages.Columns.Clear();
			gridMessages.Columns.Add(new UI.GridColumn("DateTime",140,HorizontalAlignment.Left) { SortingStrategy=UI.GridSortingStrategy.DateParse });
			gridMessages.Columns.Add(new UI.GridColumn("Sent /\r\nReceived",80,HorizontalAlignment.Center){SortingStrategy=UI.GridSortingStrategy.StringCompare} );
			gridMessages.Columns.Add(new UI.GridColumn("Status",70,HorizontalAlignment.Center) { SortingStrategy=UI.GridSortingStrategy.StringCompare });
			_idxColumnStatus=gridMessages.Columns.Count-1;
			gridMessages.Columns.Add(new UI.GridColumn("#Phone\r\nMatches",60,HorizontalAlignment.Center) { SortingStrategy=UI.GridSortingStrategy.AmountParse });
			gridMessages.Columns.Add(new UI.GridColumn("Patient\r\nPhone",100,HorizontalAlignment.Center) { SortingStrategy=UI.GridSortingStrategy.StringCompare });
			gridMessages.Columns.Add(new UI.GridColumn("Patient",150,HorizontalAlignment.Left) { SortingStrategy=UI.GridSortingStrategy.StringCompare });
			gridMessages.Columns.Add(new UI.GridColumn("Cost",32,HorizontalAlignment.Right) { SortingStrategy=UI.GridSortingStrategy.AmountParse });
			if(PrefC.HasClinicsEnabled) {
				gridMessages.Columns.Add(new UI.GridColumn("Clinic",130,HorizontalAlignment.Left) { SortingStrategy=UI.GridSortingStrategy.StringCompare });
			}
			if(checkHidden.Checked) {
				gridMessages.Columns.Add(new UI.GridColumn("Hidden",46,HorizontalAlignment.Center){SortingStrategy=UI.GridSortingStrategy.StringCompare});
			}
			for(int i=0;i<_listSmsFromMobiles.Count;i++) {
				if(!checkHidden.Checked && _listSmsFromMobiles[i].IsHidden) {
					continue;
				}
				UI.GridRow row=new UI.GridRow();
				row.Tag=_listSmsFromMobiles[i];
				if(_listSmsFromMobiles[i].SmsStatus==SmsFromStatus.ReceivedUnread) {
					row.Bold=true;
				}
				row.Cells.Add(_listSmsFromMobiles[i].DateTimeReceived.ToString());//DateTime
				row.Cells.Add(Lan.g(this,"Received"));//Type
				row.Cells.Add(SmsFromMobiles.GetSmsFromStatusDescript(_listSmsFromMobiles[i].SmsStatus));//Status
				row.Cells.Add(_listSmsFromMobiles[i].MatchCount.ToString());//#Phone Matches
				row.Cells.Add(_listSmsFromMobiles[i].MobilePhoneNumber);//Patient Phone
				row.Cells.Add(_listSmsFromMobiles[i].PatNum==0 ? Lan.g(this,"Unassigned") : GetPatientName(_listSmsFromMobiles[i].PatNum));//Patient
				row.Cells.Add("0.00");//Cost
				if(PrefC.HasClinicsEnabled) {
					if(_listSmsFromMobiles[i].ClinicNum==0) {
						row.Cells.Add(PrefC.GetString(PrefName.PracticeTitle)+" ("+Lan.g(this,"Practice")+")");
					}
					else { 
						Clinic clinic=Clinics.GetClinic(_listSmsFromMobiles[i].ClinicNum);
						row.Cells.Add(clinic.Abbr);//Clinic
					}
				}
				if(checkHidden.Checked) {
					row.Cells.Add(_listSmsFromMobiles[i].IsHidden?"X":"");//Hidden
				}
				if(patNumSelected!=0 && _listSmsFromMobiles[i].PatNum==patNumSelected) {
					row.ColorBackG=_colorSelect;
				}
				gridMessages.ListGridRows.Add(row);
			}
			if(checkSent.Checked) {
				for(int i=0;i<_listSmsToMobiles.Count;i++) {
					if(!checkHidden.Checked && _listSmsToMobiles[i].IsHidden) {
						continue;
					}
					UI.GridRow row=new UI.GridRow();
					row.Tag=_listSmsToMobiles[i];
					row.Cells.Add(_listSmsToMobiles[i].DateTimeSent.ToString());//DateTime
					row.Cells.Add(Lan.g(this,"Sent"));//Type
					string smsStatus=_listSmsToMobiles[i].SmsStatus.ToString(); //Default to the actual status.
					switch(_listSmsToMobiles[i].SmsStatus) {
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
					row.Cells.Add(_listSmsToMobiles[i].MobilePhoneNumber);//Patient Phone
					row.Cells.Add(_listSmsToMobiles[i].PatNum==0 ? "" : GetPatientName(_listSmsToMobiles[i].PatNum));//Patient
					row.Cells.Add(_listSmsToMobiles[i].MsgChargeUSD.ToString("f"));//Cost
					if(PrefC.HasClinicsEnabled) {
						if(_listSmsToMobiles[i].ClinicNum==0) {
							row.Cells.Add(PrefC.GetString(PrefName.PracticeTitle)+" ("+Lan.g(this,"Practice")+")");
						}
						else { 
							Clinic clinic=Clinics.GetClinic(_listSmsToMobiles[i].ClinicNum);
							row.Cells.Add(clinic.Abbr);//Clinic
						}
					}
					if(checkHidden.Checked) {
						row.Cells.Add(_listSmsToMobiles[i].IsHidden?"X":"");//Hidden
					}
					if(patNumSelected!=0 && _listSmsToMobiles[i].PatNum==patNumSelected) {
						row.ColorBackG=_colorSelect;
					}
					gridMessages.ListGridRows.Add(row);
				}
			}
			gridMessages.EndUpdate();
			gridMessages.SortForced(idxColSortBy,isSortAsc);
			//Check new grid rows against previous selection and re-select.			
			for(int i=0;i<gridMessages.ListGridRows.Count;i++) { 
				if(gridMessages.ListGridRows[i].Tag is SmsFromMobile && smsFromMobileSelected!=null
					&& ((SmsFromMobile)gridMessages.ListGridRows[i].Tag).SmsFromMobileNum==smsFromMobileSelected.SmsFromMobileNum) 
				{
					gridMessages.SetSelected(i,true);
					break;
				}
				if(gridMessages.ListGridRows[i].Tag is SmsToMobile && smsToMobileSelected!=null
					&& ((SmsToMobile)gridMessages.ListGridRows[i].Tag).SmsToMobileNum==smsToMobileSelected.SmsToMobileNum) 
				{
					gridMessages.SetSelected(i,true);
					break;
				}
			}
			FillGridMessageThread();
		}

		string GetDeliverStatus(SmsDeliveryStatus smsDeliveryStatus) {
			switch(smsDeliveryStatus) {
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
			return smsDeliveryStatus.ToString();
		}

		private void FillGridTextMessagesGroupedYes(int idxColSortBy,bool isSortAsc,long patNumSelected,TextMessageGrouped textMessageGroupedSelected) {
			gridMessages.Title=Lan.g(this,"Text Messages - Grouped by "+(radioGroupByPatient.Checked ? "patient" : "phone number"));
			gridMessages.BeginUpdate();
			gridMessages.ListGridRows.Clear();
			gridMessages.Columns.Clear();
			gridMessages.Columns.Add(new UI.GridColumn("DateTime",140,HorizontalAlignment.Left) { SortingStrategy=UI.GridSortingStrategy.DateParse });
			gridMessages.Columns.Add(new UI.GridColumn("Status",100,HorizontalAlignment.Left) { SortingStrategy=UI.GridSortingStrategy.StringCompare });
			_idxColumnStatus=gridMessages.Columns.Count-1;
			gridMessages.Columns.Add(new UI.GridColumn("Patient\r\nPhone",100,HorizontalAlignment.Center) { SortingStrategy=UI.GridSortingStrategy.StringCompare });
			gridMessages.Columns.Add(new UI.GridColumn("Patient",150,HorizontalAlignment.Left) { SortingStrategy=UI.GridSortingStrategy.StringCompare });
			if(PrefC.HasClinicsEnabled) {
				gridMessages.Columns.Add(new UI.GridColumn("Clinic",130,HorizontalAlignment.Left) { SortingStrategy=UI.GridSortingStrategy.StringCompare });
			}
			gridMessages.Columns.Add(new UI.GridColumn("Latest Message",150,HorizontalAlignment.Left) { SortingStrategy=UI.GridSortingStrategy.StringCompare });			
			List<TextMessageGrouped> listTextMessageGrouped=GetMessageGroups();
			for(int i=0;i<listTextMessageGrouped.Count;i++) {
				UI.GridRow row=new UI.GridRow();
				row.Tag=listTextMessageGrouped[i];
				if(listTextMessageGrouped[i].HasUnread) {
					row.Bold=true;
				}
				row.Cells.Add(listTextMessageGrouped[i].DateTimeMostRecent.ToString());
				row.Cells.Add(listTextMessageGrouped[i].Status);
				row.Cells.Add(listTextMessageGrouped[i].PatPhone);//Patient Phone
				row.Cells.Add(listTextMessageGrouped[i].PatName);
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(listTextMessageGrouped[i].ClinicAbbr);
				}
				row.Cells.Add(listTextMessageGrouped[i].TextMsg);
				gridMessages.ListGridRows.Add(row);
			}
			gridMessages.EndUpdate();
			gridMessages.SortForced(idxColSortBy,isSortAsc);
			//Check new grid rows against previous selection and re-select.			
			for(int i=0;i<gridMessages.ListGridRows.Count;i++) {
				if(
					gridMessages.ListGridRows[i].Tag is TextMessageGrouped && 
					textMessageGroupedSelected!=null &&
					((TextMessageGrouped)gridMessages.ListGridRows[i].Tag).PatPhone==textMessageGroupedSelected.PatPhone &&
					((TextMessageGrouped)gridMessages.ListGridRows[i].Tag).PatNum==textMessageGroupedSelected.PatNum)
				{
					gridMessages.SetSelected(i,true);
					break;
				}
			}
			FillGridMessageThread();
		}

		///<summary>Gets messages groups to display in the grid for when group by 'Patient' or 'Phone Number' is selected.</summary>
		private List<TextMessageGrouped> GetMessageGroups() {
			List<TextMessageGrouped> listTextMessageGroupedsToMobile=new List<TextMessageGrouped>();
			List<TextMessageGrouped> listTextMessageGroupedsFromMobile=_listSmsFromMobiles
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
					ListSmsToMobiles=new List<SmsToMobile>(),
					ListSmsFromMobiles=x.ToList(),
				}).ToList();
			if(checkSent.Checked) {
				listTextMessageGroupedsToMobile=_listSmsToMobiles
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
						ListSmsFromMobiles=new List<SmsFromMobile>(),
						ListSmsToMobiles=x.ToList(),
					}).ToList();
			}
			List<TextMessageGrouped> listTextMessageGroupedAll=listTextMessageGroupedsFromMobile.Union(listTextMessageGroupedsToMobile)
				.OrderByDescending(x => x.DateTimeMostRecent)
				//Assigning all groups a PatNum of 0 will effectively group them by phone number only
				.GroupBy(x => new { x.PatPhone,PatNum=(radioGroupByPatient.Checked ? x.PatNum : 0) })
				.Select(x => {
					//First entry from the union.
					TextMessageGrouped textMessageGrouped=x.First();
					//Patch the list back together
					textMessageGrouped.HasUnread=x.Any(y => y.HasUnread);
					textMessageGrouped.ListSmsToMobiles=x.SelectMany(y => y.ListSmsToMobiles).ToList();
					textMessageGrouped.ListSmsFromMobiles=x.SelectMany(y => y.ListSmsFromMobiles).ToList();
					return textMessageGrouped;
				}).ToList();
			return listTextMessageGroupedAll;
		}		

		private void FillGridMessageThread() {
			if(GetSelectedPatNum()==0) {
				Text=Lan.g(this,"Text Messaging");
			}
			else {
				Text=Lan.g(this,"Text Messaging - Patient:")+" "+GetPatientName(GetSelectedPatNum());
			}
			labelPatientsForPhone.Visible=false;
			if(GetSelectedPatNum()==0 && GetSelectedSmsGroup()==null) { //A message with no patNum was selected and is not a group, or nothing is selected at all.
				if(GetSelectedSmsToMobile()!=null) { //SmsToMobile is selected.
					smsThreadView.ListSmsThreadMessages=new List<SmsThreadMessage>() { new SmsThreadMessage("T"+GetSelectedSmsToMobile().SmsToMobileNum,GetSelectedSmsToMobile().DateTimeSent,GetSelectedSmsToMobile().MsgText,true,true,true,GetSelectedSmsToMobile().SmsStatus) };
				}
				else if(GetSelectedSmsFromMobile()!=null) { //SmsFromMobile is selected.
					smsThreadView.ListSmsThreadMessages=new List<SmsThreadMessage>() { new SmsThreadMessage("F"+GetSelectedSmsFromMobile().SmsFromMobileNum,GetSelectedSmsFromMobile().DateTimeReceived,GetSelectedSmsFromMobile().MsgText,true,true,true,SmsDeliveryStatus.None) };
				}
				else { //Nothing selected at all, clear the message thread.
					smsThreadView.ListSmsThreadMessages=null;
				}
				SetReplyHelper();
				return;
			}
			List<SmsThreadMessage> listSmsThreadMessages=new List<SmsThreadMessage>();
			long patNumForDb;
			string strNumberForDb;
			if(radioGroupByPhone.Checked) {//Grouped by phone. Get all messages from the database for the given phone number. Don't specify patNum.
				patNumForDb=-1;
				strNumberForDb=GetSelectedMobileNumber();
			}
			else if(GetSelectedPatNum()==0 && radioGroupByPatient.Checked) {//Grouped by Patient and the patient's patnum is 0. Find messages with the patnum/number combo.
				patNumForDb=GetSelectedPatNum();
				strNumberForDb=GetSelectedMobileNumber();
			}
			else {//Normal patnum. Find all messages for the patnum regardless of number.
				patNumForDb=GetSelectedPatNum();
				strNumberForDb="";
			}
			List<SmsFromMobile> listSmsFromMobile=SmsFromMobiles.GetMessages(DateTime.MinValue,DateTime.MinValue,GetListSelectedClinicNums(),
				patNumForDb,true,strNumberForDb,new List<SmsFromStatus>());
			for(int i=0;i<listSmsFromMobile.Count;i++) {
				bool isHighlighted=false;
				if(GetSelectedSmsFromMobile()!=null && GetSelectedSmsFromMobile().SmsFromMobileNum==listSmsFromMobile[i].SmsFromMobileNum) {
					isHighlighted=true;
				}
				listSmsThreadMessages.Add(new SmsThreadMessage("F"+listSmsFromMobile[i].SmsFromMobileNum,listSmsFromMobile[i].DateTimeReceived,listSmsFromMobile[i].MsgText,true,false,isHighlighted,SmsDeliveryStatus.None));
			}
			List<SmsToMobile> listSmsToMobile=SmsToMobiles.GetMessages(DateTime.MinValue,DateTime.MinValue,GetListSelectedClinicNums(),patNumForDb,
				strNumberForDb);
			for(int i=0;i<listSmsToMobile.Count;i++) {
				bool isHighlighted=false;
				if(GetSelectedSmsToMobile()!=null && GetSelectedSmsToMobile().SmsToMobileNum==listSmsToMobile[i].SmsToMobileNum) {
					isHighlighted=true;
				}
				bool isImportant=false;
				if(listSmsToMobile[i].SmsStatus==SmsDeliveryStatus.FailNoCharge ||listSmsToMobile[i].SmsStatus==SmsDeliveryStatus.FailWithCharge) {
					isImportant=true;
				}
				listSmsThreadMessages.Add(new SmsThreadMessage("T"+listSmsToMobile[i].SmsToMobileNum,listSmsToMobile[i].DateTimeSent,listSmsToMobile[i].MsgText,false,isImportant,isHighlighted,listSmsToMobile[i].SmsStatus));
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
			if(!HasSelectedMessage()) {
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
			_patNum=(FormOpenDental.PatNumCur==0 ? -1 : FormOpenDental.PatNumCur);
			if(_patNum==-1) {
				textPatient.Text="";
			}
			else {
				textPatient.Text=Patients.GetLim(_patNum).GetNameLF();
			}
		}

		private void butPatFind_Click(object sender,EventArgs e) {
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.ShowDialog();
			if(formPatientSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			_patNum=formPatientSelect.PatNumSelected;
			textPatient.Text=Patients.GetLim(_patNum).GetNameLF();
		}

		private void butPatAll_Click(object sender,EventArgs e) {
			_patNum=-1;
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
			List<SmsFromMobile> listSmsFromMobilesMarkReceivedRead=new List<SmsFromMobile>();
			//If the clicked/selected message was a ReceivedUnread message, then mark the message ReceivedRead in the db as well as the grid.
			if(GetSelectedSmsFromMobile()!=null && GetSelectedSmsFromMobile().SmsStatus==SmsFromStatus.ReceivedUnread) {
				listSmsFromMobilesMarkReceivedRead.Add(GetSelectedSmsFromMobile());
			}
			if(GetSelectedSmsGroup()!=null) {
				listSmsFromMobilesMarkReceivedRead.AddRange(GetSelectedSmsGroup().ListSmsFromMobiles.FindAll(x => x.SmsStatus==SmsFromStatus.ReceivedUnread));
			}
			if(listSmsFromMobilesMarkReceivedRead.Count>0) {
				//Notifier is intentionally invoked before updating the sms row. This will allow local count to be modified without the risk of a race condition.			
				_actionSmsNotifier?.Invoke(-listSmsFromMobilesMarkReceivedRead.Count);
				//Messages were marked as 'Read' so update the db and update the grid in place.
				for(int i=0;i<listSmsFromMobilesMarkReceivedRead.Count;i++) {
					SmsFromMobile smsFromMobileOld=listSmsFromMobilesMarkReceivedRead[i].Copy();
					listSmsFromMobilesMarkReceivedRead[i].SmsStatus=SmsFromStatus.ReceivedRead;
					SmsFromMobiles.Update(listSmsFromMobilesMarkReceivedRead[i],smsFromMobileOld);
				}
				//Fix the rows in place. Forcing an entire refresh would cause sorting of grid not to persist.
				gridMessages.ListGridRows[gridMessages.SelectedIndices[0]].Bold=false;
				if(gridMessages.ListGridRows[gridMessages.SelectedIndices[0]].Tag is TextMessageGrouped textMessageGrouped) {
					textMessageGrouped.HasUnread=false;
					//Latest entry wins for this column value.
					SmsFromMobile smsFromMobileLatest=textMessageGrouped.ListSmsFromMobiles.OrderByDescending(x => x.DateTimeReceived).FirstOrDefault()??new SmsFromMobile() { DateTimeReceived=DateTime.MinValue };
					SmsToMobile smsToMobileLatest=textMessageGrouped.ListSmsToMobiles.OrderByDescending(x => x.DateTimeSent).FirstOrDefault()??new SmsToMobile() {DateTimeSent=DateTime.MinValue };
					gridMessages.ListGridRows[gridMessages.SelectedIndices[0]].Cells[_idxColumnStatus].Text=
						smsFromMobileLatest.DateTimeReceived>smsToMobileLatest.DateTimeSent ? 
							Lan.g(this,"Rcv")+" - "+SmsFromMobiles.GetSmsFromStatusDescript(SmsFromStatus.ReceivedRead) : 
							Lan.g(this,"Sent")+" - "+GetDeliverStatus(smsToMobileLatest.SmsStatus);
				}
				else if(gridMessages.ListGridRows[gridMessages.SelectedIndices[0]].Tag is SmsFromMobile smsFromMobile) {
					smsFromMobile.SmsStatus=SmsFromStatus.ReceivedRead;
					gridMessages.ListGridRows[gridMessages.SelectedIndices[0]].Cells[_idxColumnStatus].Text=SmsFromMobiles.GetSmsFromStatusDescript(SmsFromStatus.ReceivedRead);
				}
			}
			//Update highlighted rows.
			long selectedPatNum=GetSelectedPatNum();
			for(int i=0;i<gridMessages.ListGridRows.Count;i++){
				long patNum=0;
				if(gridMessages.ListGridRows[i].Tag is TextMessageGrouped) {
					patNum=((TextMessageGrouped)gridMessages.ListGridRows[i].Tag).PatNum;
				}
				else if(gridMessages.ListGridRows[i].Tag is SmsFromMobile) {
					patNum=((SmsFromMobile)gridMessages.ListGridRows[i].Tag).PatNum;
				}
				else if(gridMessages.ListGridRows[i].Tag is SmsToMobile) {
					patNum=((SmsToMobile)gridMessages.ListGridRows[i].Tag).PatNum;
				}
				gridMessages.ListGridRows[i].ColorBackG=patNum!=0 && selectedPatNum==patNum ? _colorSelect : Color.White;
			}
			gridMessages.Invalidate();
			FillGridMessageThread();
		}
		
		///<summary>Sets the given status for the selected receieved message.</summary>
		private void SetReceivedSelectedStatus(SmsFromStatus smsFromStatus) {
			if(GetSelectedSmsGroup()!=null) {
				MsgBox.Show(this,"Please turn off Group By Patient.");
				return;
			}
			if(GetSelectedSmsFromMobile()==null) {
				MsgBox.Show(this,"Please select a received message.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			UI.GridRow row=gridMessages.ListGridRows[gridMessages.GetSelectedIndex()];
			SmsFromMobile smsFromMobile=GetSelectedSmsFromMobile();
			SmsFromMobile smsFromMobileOld=smsFromMobile.Copy();
			smsFromMobile.SmsStatus=smsFromStatus;
			//Notifier is intentionally invoked before updating the sms row. This will allow local count to be modified without the risk of a race condition.
			_actionSmsNotifier?.Invoke(smsFromStatus==SmsFromStatus.ReceivedRead ? -1 : 1);
			SmsFromMobiles.Update(smsFromMobile,smsFromMobileOld);
			row.Cells[_idxColumnStatus].Text=SmsFromMobiles.GetSmsFromStatusDescript(smsFromStatus);
			row.Bold=false;
			if(smsFromStatus==SmsFromStatus.ReceivedUnread) {
				row.Bold=true;
			}			
			gridMessages.Invalidate();//To show the status changes in the grid.
			Cursor=Cursors.Default;
		}

		private void menuItemChangePat_Click(object sender,EventArgs e) {
			if(GetSelectedSmsGroup()!=null) {
				MsgBox.Show(this,"Please turn off Group By Patient.");
				return;
			}
			if(GetSelectedSmsFromMobile()==null) {
				MsgBox.Show(this,"Please select a received message.");
				return;
			}
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			List<Patient> listPatients=Patients.GetPatientsByPhone(GetSelectedSmsFromMobile().MobilePhoneNumber,				
				CultureInfo.CurrentCulture.Name.Substring(CultureInfo.CurrentCulture.Name.Length-2));//Country code of current environment.
			if(Clinics.ClinicNum!=0) {
				List<long> listClinicNums=GetListSelectedClinicNums().Union(new List<long>() { GetSelectedSmsFromMobile().ClinicNum }).ToList();
				listPatients=listPatients.Where(x => listClinicNums.Contains(x.ClinicNum)).ToList();
			}
			formPatientSelect.ListPatNumsExplicit=listPatients.Select(x=> x.PatNum).ToList();
			if(formPatientSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			SmsFromMobile smsFromMobile=GetSelectedSmsFromMobile();
			SmsFromMobile smsFromMobileOld=smsFromMobile.Copy();
			smsFromMobile.PatNum=formPatientSelect.PatNumSelected;
			SmsFromMobiles.Update(smsFromMobile,smsFromMobileOld);
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
				Commlog commlogOld=commlog.Copy();
				commlog.PatNum=formPatientSelect.PatNumSelected;
				Commlogs.Update(commlog,commlogOld);
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
			if(!HasSelectedMessage()) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			if(GetSelectedSmsFromMobile()!=null) {
				SmsFromMobile smsFromMobileOld=GetSelectedSmsFromMobile().Copy();
				GetSelectedSmsFromMobile().IsHidden=isHide;
				SmsFromMobiles.Update(GetSelectedSmsFromMobile(),smsFromMobileOld);
			}
			else if(GetSelectedSmsToMobile()!=null) {
				SmsToMobile smsToMobileOld=GetSelectedSmsToMobile().Copy();
				GetSelectedSmsToMobile().IsHidden=isHide;
				SmsToMobiles.Update(GetSelectedSmsToMobile(),smsToMobileOld);
			}
			Cursor=Cursors.Default;
			FillGridTextMessages();
		}

		private void menuItemHide_Click(object sender,EventArgs e) {
			if(!HasSelectedMessage()) {
				MsgBox.Show(this,"Please select a message before attempting to hide.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Hide selected message?")) {
				return;
			}
			HideOrUnhideMessages(true);
		}

		private void menuItemUnhide_Click(object sender,EventArgs e) {
			if(!HasSelectedMessage()) {
				MsgBox.Show(this,"Please select a message before attempting to unhide.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Unhide selected message?")) {
				return;
			}
			HideOrUnhideMessages(false);
		}

		private void menuItemBlockNumber_Click(object sender,EventArgs e) {
			if(!HasSelectedMessage()) {
				MsgBox.Show(this,"Please select a received or sent message to block.");
				return;
			}
			string strNumberToBlock=GetSelectedMobileNumber();
			string question=Lan.g(this,"Block incoming texts from")+" "+strNumberToBlock+"? "+Lan.g(this,"This cannot be undone.");
			if(GetSelectedPatNum()!=0) {
				Patient pat=Patients.GetPat(GetSelectedPatNum());
				if(pat!=null) {
					question+="\r\n"+Lan.g(this,"This phone number is attached to patient")+" "+pat.GetNameFLnoPref()+".";
				}
			}
			if(MessageBox.Show(question,"",MessageBoxButtons.YesNo)==DialogResult.No) {
				return;
			}
			SmsBlockPhones.Insert(new SmsBlockPhone { BlockWirelessNumber=strNumberToBlock });
			DataValid.SetInvalid(InvalidType.SmsBlockPhones);
		}
				
		private void menuItemSelectPatient_Click(object sender,EventArgs e) {
			if(!HasSelectedMessage()) {
				MsgBox.Show(this,"Please select a message first.");
				return;
			}			
			if(GetSelectedPatNum()==0 || GetPatientName(GetSelectedPatNum())==Lan.g(this,"Not found")) {//If the patNum is 0 or patient could not be found.
				MsgBox.Show(this,"Please select a message with a valid patient attached.");
				return;
			}
			FormOpenDental.S_Contr_PatientSelected(Patients.GetPat(GetSelectedPatNum()),true);
		}

		private void butSend_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.TextMessageSend)) {
				return;
			}
			if(!HasSelectedMessage()) {
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
			else if(GetSelectedSmsGroup()!=null) {
				clinicNum=GetSelectedSmsGroup().ClinicNum;//can be 0
			}
			else if(GetSelectedSmsFromMobile()!=null) {
				clinicNum=GetSelectedSmsFromMobile().ClinicNum;//can be 0
			}
			else if(GetSelectedSmsToMobile()!=null) {
				clinicNum=GetSelectedSmsToMobile().ClinicNum;//can be 0
			}
			if(string.IsNullOrEmpty(GetSelectedMobileNumber())) {
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
				GridRow row=gridMessages.ListGridRows[i];
				if(row.ColorBackG!=_colorSelect) { //Verify that the row the user appears to have selected is actually the selected row.
					continue;
				}
				if(row.Tag is TextMessageGrouped) {
					TextMessageGrouped smsGroup=(TextMessageGrouped)row.Tag;
					if(smsGroup.PatNum!=GetSelectedPatNum()) {
						gridMessages.SetSelected(i,true);//Change the selection to the first highlighted row.
						isInvalidSelection=true;
						break;
					}
				}
				else if(row.Tag is SmsFromMobile) {
					SmsFromMobile smsFrom=(SmsFromMobile)row.Tag;
					if(smsFrom.PatNum!=GetSelectedPatNum()) {
						gridMessages.SetSelected(i,true);//Change the selection to the first highlighted row.
						isInvalidSelection=true;
						break;
					}
				}
				else if(row.Tag is SmsToMobile) {
					SmsToMobile smsTo=(SmsToMobile)row.Tag;
					if(smsTo.PatNum!=GetSelectedPatNum()) {
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
				if(Plugins.HookMethod(this,"FormSmsTextMessaging.butReply_Click_sendSmsSingle",GetSelectedPatNum(),GetSelectedMobileNumber(),textReply.Text,YN.Yes)) {
					goto HookSkipSmsCall;
				}
				SmsToMobiles.SendSmsSingle(GetSelectedPatNum(),GetSelectedMobileNumber(),textReply.Text,clinicNum,SmsMessageSource.DirectSms,user: Security.CurUser);
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
			public List<SmsFromMobile> ListSmsFromMobiles=new List<SmsFromMobile>();
			public List<SmsToMobile> ListSmsToMobiles=new List<SmsToMobile>();
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