using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class DashApptGrid:UserControl,IDashWidgetField {
		//if the name of this changes, it also will need to be changed manually in OpenDentBusiness.SheetUtil.GetGridColumnsAvailable:353
		public const string SheetFieldName="ApptsGrid";
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();

		public Patient PatCur;
		private List<ApptOther> _listApptOthers=new List<ApptOther>();
		private List<PlannedAppt> _listPlannedAppts=new List<PlannedAppt>();
		private List<PlannedAppt> _listPlannedIncompletes=new List<PlannedAppt>();
		private List<Def> _listProgNoteColorDefs=new List<Def>();
		private Action _actionFillFamily;
		public bool IsShowCompletePlanned;
		private bool _isInDashboard;
		///<summary>Indicates the appointment grid is executing a full refresh, ie queries are run.  Used to scroll the grid to the end when the Patient
		///Dashboard is fully refreshed, which occurs when the selected patient changes or the user clicks "Refresh".</summary>
		private bool _isFullRefresh;

		///<summary>Returns the selected ApptOther.  Returns null if no ApptOther is selected.</summary>
		public ApptOther SelectedApptOther {
			get {
				if(gridMain.GetSelectedIndex()==-1) {
					return null;
				}
				return _listApptOthers.FirstOrDefault(x => x.AptNum==gridMain.SelectedTag<long>());
			}
		}

		public List<ApptOther> ListApptOthers {
			get {
				return _listApptOthers;
			}
		}

		public DashApptGrid() {
			InitializeComponent();
		}

		public void PassLayoutManager(LayoutManagerForms layoutManager){
			LayoutManager=layoutManager;
		}

		private void DashApptGrid_Load(object sender,EventArgs e) {
			AppointmentEvent.Fired+=AppointmentEvent_Fired;//Only subscribe to this event if actually showing.
			//Need to be able to unsubscribe when the Parent's handle is destroyed, otherwise this subscription sticks around, i.e. memory leak that will 
			//cause the AppointmentEvent_Fired handler to run for the Parent even though it's already "closed" (though not actually disposed due to this 
			//subscription).
			this.Parent.HandleDestroyed+=UnsubscribeApptEvent;
			this.Disposed+=UnsubscribeApptEvent;
			gridMain.ScaleMy=LayoutManager.ScaleMy();
		}

		private void DashApptGrid_SizeChanged(object sender,EventArgs e) {
			gridMain.SetBounds(0,0,Width,Height);
		}

		private void UnsubscribeApptEvent(object sender,EventArgs e) {
			AppointmentEvent.Fired-=AppointmentEvent_Fired;
		}

		private void AppointmentEvent_Fired(ODEventArgs e) {
			if(PatCur==null) {
				return;
			}
			bool isRefreshRequired=false;
			List<Appointment> listAppts=new List<Appointment>();
			if(e.Tag is Appointment) {
				listAppts.Add((Appointment)e.Tag);
			}
			else if(e.Tag is List<Appointment>) {
				listAppts=(List<Appointment>)e.Tag;
			}
			else {
				return;//Event fired with unexpected Tag.
			}
			foreach(Appointment appt in listAppts) {
				if(appt.PatNum==PatCur.PatNum) {
					isRefreshRequired=true;
					break;
				}
			}
			if(isRefreshRequired) {
				//_isInDashboard flag will already be set by this point which is used for enabling/disabling the vertical scroll bar when this control is 
				//used in the Patient Dashboard, so we don't care if we pass a sheetField or not.
				RefreshData(PatCur,null);
				RefreshView();
			}
		}

		public void SetFillFamilyAction(Action action) {
			_actionFillFamily=action;
		}

		public void ScrollToEnd() {
			gridMain.ScrollToEnd();
		}

		public void RefreshAppts() {
			RefreshData(PatCur,null);
			RefreshView();
		}

		public void SetData(PatientDashboardDataEventArgs data,SheetField sheetField) {
			if(!IsNecessaryDataAvailable(data)) {
				return;
			}
			_listApptOthers=data.ListAppts.FindAll(x => x.PatNum==data.Pat.PatNum).Select(y => new ApptOther(y)).ToList();;
			_listPlannedAppts=data.ListPlannedAppts;
			_listPlannedIncompletes=_listPlannedAppts.FindAll(x => !_listApptOthers.ToList()
				.Exists(y => y.NextAptNum==x.AptNum && y.AptStatus==ApptStatus.Complete))
				.OrderBy(x => x.ItemOrder).ToList();
			_listProgNoteColorDefs=Defs.GetDefsForCategory(DefCat.ProgNoteColors);//cached
		}

		private bool IsNecessaryDataAvailable(PatientDashboardDataEventArgs data) {
			if(data.ListPlannedAppts==null || data.ListAppts==null || data.Pat==null) {
				return false;//Necessary data not found in PatientDashboardDataEventArgs.  Make no change.
			}
			return true;
		}

		public void RefreshData(Patient pat,SheetField sheetField) {
			PatCur=pat;
			if(sheetField!=null) {
				_isInDashboard=true;
			}
			if(PatCur==null) {
				return;
			}
			_listApptOthers=Appointments.GetApptOthersForPat(PatCur.PatNum);
			_listPlannedAppts=PlannedAppts.Refresh(PatCur.PatNum);
			_listPlannedIncompletes=_listPlannedAppts.FindAll(x => !_listApptOthers.ToList()
				.Exists(y => y.NextAptNum==x.AptNum && y.AptStatus==ApptStatus.Complete))
				.OrderBy(x => x.ItemOrder).ToList();
			_listProgNoteColorDefs=Defs.GetDefsForCategory(DefCat.ProgNoteColors);
			_isFullRefresh=true;
		}

		public void RefreshView() {
			if(_isInDashboard) {
				//Enable horizontal scrolling so call to ODGrid.ComputeColumns() does not disable vertical scrolling when in the PatientDashboard.
				gridMain.HScrollVisible=true;
			}
			FillGrid();
			if(Parent.Width<gridMain.Width || Width<gridMain.ListGridColumns.Sum(x => x.ColWidth)) {
				gridMain.HScrollVisible=true;
			}
			else {
			}
			if(_isFullRefresh && _isInDashboard) {
				gridMain.ScrollToEnd();
				_isFullRefresh=false;
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			int currentSelection=e.Row;
			int currentScroll=gridMain.ScrollValue;
			long aptNum=gridMain.SelectedTag<long>();//Tag is AptNum
			if(IsSelectedApptOtherNull()) {
				return;
			}
			using FormApptEdit formApptEdit=new FormApptEdit(aptNum);
			formApptEdit.IsInViewPatAppts=true;
			formApptEdit.PinIsVisible=true;
			formApptEdit.ShowDialog();
			if(formApptEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			if(formApptEdit.PinClicked) {
				if(IsSelectedApptOtherNull()) {
					return;
				}
				SendToPinboardEvent.Fire(ODEventType.SendToPinboard,new PinBoardArgs(PatCur,SelectedApptOther,_listApptOthers));
			}
			else{
				RefreshData(PatCur,null);
				_actionFillFamily?.Invoke();
				FillGrid();
				gridMain.SetSelected(currentSelection,true);
				gridMain.ScrollValue=currentScroll;
			}
		}

		///<summary>Returns true if SelectedApptOther is null or is set to an appointment with an AptNum that is not in the database.
		///If SelectedApptOther was null or set to a valid appointment but is now invalid (no longer in the database) then a warning message will display
		///to the user and RefreshAppts will be invoked in order to remove the invalid appt from the UI.</summary>
		public bool IsSelectedApptOtherNull() {
			if(SelectedApptOther==null || Appointments.GetOneApt(SelectedApptOther.AptNum)==null) {
				MsgBox.Show(this,"Selected appointment no longer exists.");
				RefreshAppts();
				return true;
			}
			return false;
		}

		private void FillGrid() {
			gridMain.ScaleMy=LayoutManager.ScaleMy();
			long selectedApptOtherNum=SelectedApptOther?.AptNum??-1;
			int selectedIndex=-1;
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormApptsOther","Appt Status"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormApptsOther","Prov"),50);
			gridMain.ListGridColumns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g("FormApptsOther","Clinic"),80);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g("FormApptsOther","Date"),70);//If the order changes, reflect the change for dateIndex below.
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormApptsOther","Time"),70);//Must immediately follow Date column.
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormApptsOther","Min"),40);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormApptsOther","Procedures"),150);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormApptsOther","Notes"),320);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			int dateIndex=3;
			if(!PrefC.HasClinicsEnabled) {
				dateIndex=2;
			}
			for(int i=0;i<_listApptOthers.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listApptOthers[i].AptStatus.ToString());
				row.Cells.Add(Providers.GetAbbr(_listApptOthers[i].ProvNum));
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(_listApptOthers[i].ClinicNum));
				}
				row.Cells.Add("");//Date
				row.Cells.Add("");//Time
				if(_listApptOthers[i].AptDateTime.Year > 1880) {
					//only regular still scheduled appts
					if(_listApptOthers[i].AptStatus!=ApptStatus.Planned && _listApptOthers[i].AptStatus!=ApptStatus.PtNote 
						&& _listApptOthers[i].AptStatus!=ApptStatus.PtNoteCompleted && _listApptOthers[i].AptStatus!=ApptStatus.UnschedList 
						&& _listApptOthers[i].AptStatus!=ApptStatus.Broken) 
					{
						row.Cells[dateIndex].Text=_listApptOthers[i].AptDateTime.ToString("d");
						row.Cells[dateIndex+1].Text=_listApptOthers[i].AptDateTime.ToString("t");
						if(_listApptOthers[i].AptDateTime < DateTime.Today) { //Past
							row.ColorBackG=_listProgNoteColorDefs[11].ItemColor;
							row.ColorText=_listProgNoteColorDefs[10].ItemColor;
						}
						else if(_listApptOthers[i].AptDateTime.Date==DateTime.Today.Date) { //Today
							row.ColorBackG=_listProgNoteColorDefs[9].ItemColor;
							row.ColorText=_listProgNoteColorDefs[8].ItemColor;
							row.Cells[0].Text=Lan.g(this,"Today");
						}
						else if(_listApptOthers[i].AptDateTime > DateTime.Today) { //Future
							row.ColorBackG=_listProgNoteColorDefs[13].ItemColor;
							row.ColorText=_listProgNoteColorDefs[12].ItemColor;
						}
					}
					else if(_listApptOthers[i].AptStatus==ApptStatus.Planned) { //show line for planned appt
						row.ColorBackG=_listProgNoteColorDefs[17].ItemColor;
						row.ColorText=_listProgNoteColorDefs[16].ItemColor;
						string txt=Lan.g("enumApptStatus","Planned")+" ";
						int plannedAptIdx=_listPlannedIncompletes.FindIndex(x => x.AptNum==_listApptOthers[i].AptNum);
						if(IsShowCompletePlanned) {
							for(int p=0;p<_listPlannedAppts.Count;p++) {
								if(_listPlannedAppts[p].AptNum==_listApptOthers[i].AptNum) {
									txt+="#"+_listPlannedAppts[p].ItemOrder.ToString();
								}
							}
						}
						else {
							if(plannedAptIdx>=0) {
								txt+="#"+(plannedAptIdx+1);
							}
							else {
								continue;
							}
						}
						if(plannedAptIdx<0) {//attached to a completed appointment
							txt+=" ("+Lan.g("enumApptStatus",ApptStatus.Complete.ToString())+")";
						}
						if(_listApptOthers.ToList().FindAll(x => x.NextAptNum==_listApptOthers[i].AptNum)
							.Exists(x => x.AptStatus==ApptStatus.Scheduled)) //attached to a scheduled appointment
						{
							txt+=" ("+Lan.g("enumApptStatus",ApptStatus.Scheduled.ToString())+")";
						}
						row.Cells[0].Text=txt;
					}
					else if(_listApptOthers[i].AptStatus==ApptStatus.PtNote) {
						row.ColorBackG=_listProgNoteColorDefs[19].ItemColor;
						row.ColorText=_listProgNoteColorDefs[18].ItemColor;
						row.Cells[0].Text=Lan.g("enumApptStatus","PtNote");
					}
					else if(_listApptOthers[i].AptStatus==ApptStatus.PtNoteCompleted) {
						row.ColorBackG=_listProgNoteColorDefs[21].ItemColor;
						row.ColorText=_listProgNoteColorDefs[20].ItemColor;
						row.Cells[0].Text=Lan.g("enumApptStatus","PtNoteCompleted");
					}
					else if(_listApptOthers[i].AptStatus==ApptStatus.Broken) {
						row.Cells[0].Text=Lan.g("enumApptStatus","Broken");
						row.Cells[dateIndex].Text=_listApptOthers[i].AptDateTime.ToString("d");
						row.Cells[dateIndex+1].Text=_listApptOthers[i].AptDateTime.ToString("t");
						row.ColorBackG=_listProgNoteColorDefs[15].ItemColor;
						row.ColorText=_listProgNoteColorDefs[14].ItemColor;
					}
					else if(_listApptOthers[i].AptStatus==ApptStatus.UnschedList) {
						row.Cells[0].Text=Lan.g("enumApptStatus","UnschedList");
						row.ColorBackG=_listProgNoteColorDefs[15].ItemColor;
						row.ColorText=_listProgNoteColorDefs[14].ItemColor;
					}
				}
				row.Cells.Add((_listApptOthers[i].Pattern.Length * 5).ToString());
				row.Cells.Add(_listApptOthers[i].ProcDescript);
				row.Cells.Add(_listApptOthers[i].Note);
				row.Tag=_listApptOthers[i].AptNum;
				gridMain.ListGridRows.Add(row);
				if((long)row.Tag==selectedApptOtherNum) {
					selectedIndex=i;
				}
			}
			gridMain.EndUpdate();
			if(selectedIndex>-1) {
				gridMain.SetSelected(selectedIndex,true);
			}
		}

		
	}

	public class PinBoardArgs {
		public ApptOther ApptOther;
		public List<ApptOther> ListApptOthers;
		public Patient Pat;

		public PinBoardArgs(Patient pat,ApptOther apptOther,List<ApptOther> listApptOthers) {
			Pat=pat;
			ApptOther=apptOther;
			ListApptOthers=listApptOthers;
		}
	}
}
