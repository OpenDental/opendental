using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		public Patient PatientCur;
		public List<ApptOther> ListApptOthers=new List<ApptOther>();
		private List<PlannedAppt> _listPlannedAppts=new List<PlannedAppt>();
		private List<PlannedAppt> _listPlannedApptsIncomplete=new List<PlannedAppt>();
		private List<Def> _listDefsProgNoteColors=new List<Def>();
		public bool IsShowCompletePlanned;
		private bool _isInDashboard;
		///<summary>Indicates the appointment grid is executing a full refresh, ie queries are run.  Used to scroll the grid to the end when the Patient
		///Dashboard is fully refreshed, which occurs when the selected patient changes or the user clicks "Refresh".</summary>
		private bool _isFullRefresh;

		public DashApptGrid() {
			InitializeComponent();
		}

		public void PassLayoutManager(LayoutManagerForms layoutManager){
			LayoutManager=layoutManager;
		}

		private void DashApptGrid_Load(object sender,EventArgs e) {
			ODEvent.Fired+=AppointmentEvent_Fired;//Only subscribe to this event if actually showing.
			//Need to be able to unsubscribe when the Parent's handle is destroyed, otherwise this subscription sticks around, i.e. memory leak that will 
			//cause the AppointmentEvent_Fired handler to run for the Parent even though it's already "closed" (though not actually disposed due to this 
			//subscription).
			this.Parent.HandleDestroyed+=UnsubscribeApptEvent;
			this.Disposed+=UnsubscribeApptEvent;
			gridMain.SetScaleAndZoom(LayoutManager.GetScaleMS(),LayoutManager.GetZoomLocal());
		}

		private void DashApptGrid_SizeChanged(object sender,EventArgs e) {
			gridMain.SetBounds(0,0,Width,Height);
		}

		private void UnsubscribeApptEvent(object sender,EventArgs e) {
			ODEvent.Fired-=AppointmentEvent_Fired;
		}

		private void AppointmentEvent_Fired(ODEventArgs e) {
			if(e.EventType!=ODEventType.AppointmentEdited){
				return;
			}
			if(PatientCur==null) {
				return;
			}
			bool isRefreshRequired=false;
			List<Appointment> listAppointments=new List<Appointment>();
			if(e.Tag is Appointment) {
				listAppointments.Add((Appointment)e.Tag);
			}
			else if(e.Tag is List<Appointment>) {
				listAppointments=(List<Appointment>)e.Tag;
			}
			else if(e.Tag is List<Signalod>) {
				List<Signalod> listSignalods=(List<Signalod>)e.Tag;
				for(int i=0;i<listSignalods.Count;i++) {
					if(listSignalods[i].FKeyType==KeyType.PatNum) {
						Appointment appointmentFake=new Appointment();
						appointmentFake.PatNum=listSignalods[i].FKey;
						listAppointments.Add(appointmentFake);
					}
				}
			}
			else {
				return;//Event fired with unexpected Tag.
			}
			for(int i=0;i<listAppointments.Count;i++) {
				if(listAppointments[i].PatNum==PatientCur.PatNum) {
					isRefreshRequired=true;
					break;
				}
			}
			if(isRefreshRequired) {
				//_isInDashboard flag will already be set by this point which is used for enabling/disabling the vertical scroll bar when this control is 
				//used in the Patient Dashboard, so we don't care if we pass a sheetField or not.
				RefreshData(PatientCur,null);
				RefreshView();
			}
		}

		public void SetData(PatientDashboardDataEventArgs data,SheetField sheetField) {
			if(!IsNecessaryDataAvailable(data)) {
				return;
			}
			ListApptOthers=data.ListAppts.FindAll(x => x.PatNum==data.Pat.PatNum).Select(y => new ApptOther(y)).ToList();
			_listPlannedAppts=data.ListPlannedAppts;
			_listPlannedApptsIncomplete=_listPlannedAppts.FindAll(x => !ListApptOthers.ToList()
				.Exists(y => y.NextAptNum==x.AptNum && y.AptStatus==ApptStatus.Complete))
				.OrderBy(x => x.ItemOrder).ToList();
			_listDefsProgNoteColors=Defs.GetDefsForCategory(DefCat.ProgNoteColors);//cached
		}

		private bool IsNecessaryDataAvailable(PatientDashboardDataEventArgs data) {
			if(data.ListPlannedAppts==null || data.ListAppts==null || data.Pat==null) {
				return false;//Necessary data not found in PatientDashboardDataEventArgs.  Make no change.
			}
			return true;
		}

		public void RefreshData(Patient patient,SheetField sheetField) {
			PatientCur=patient;
			if(sheetField!=null) {
				_isInDashboard=true;
			}
			if(PatientCur==null) {
				return;
			}
			ListApptOthers=Appointments.GetApptOthersForPat(PatientCur.PatNum);
			_listPlannedAppts=PlannedAppts.Refresh(PatientCur.PatNum);
			//Same ordering as gridMain in FormApptsOther.cs.
			ListApptOthers=ListApptOthers.OrderBy(appt => {
				var plannedAppt=_listPlannedAppts.FirstOrDefault(x => x.AptNum==appt.AptNum);
				return plannedAppt?.ItemOrder+ListApptOthers.Count??(ListApptOthers.IndexOf(appt));//Place planned appts after non-planned appts. Keep the same ordering amongst non-planned.
			}).ToList();
			_listPlannedApptsIncomplete=_listPlannedAppts.FindAll(x => !ListApptOthers.ToList()
				.Exists(y => y.NextAptNum==x.AptNum && y.AptStatus==ApptStatus.Complete))
				.OrderBy(x => x.ItemOrder).ToList();
			_listDefsProgNoteColors=Defs.GetDefsForCategory(DefCat.ProgNoteColors);
			_isFullRefresh=true;
		}

		public void RefreshView() {
			if(_isInDashboard) {
				//Enable horizontal scrolling so call to ODGrid.ComputeColumns() does not disable vertical scrolling when in the PatientDashboard.
				gridMain.HScrollVisible=true;
			}
			FillGrid();
			if(Parent.Width<gridMain.Width || Width<gridMain.Columns.Sum(x => x.ColWidth)) {
				gridMain.HScrollVisible=true;
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
				ODEvent.Fire(ODEventType.SendToPinboard,new PinBoardArgs(PatientCur,ListApptOthers.FirstOrDefault(x=>x.AptNum==gridMain.SelectedTag<long>()),ListApptOthers));
				return;
			}
			RefreshData(PatientCur,null);
			FillGrid();
			gridMain.SetSelected(currentSelection,true);
			gridMain.ScrollValue=currentScroll;
		}

		///<summary>Returns true if SelectedApptOther is null or is set to an appointment with an AptNum that is not in the database.
		///If SelectedApptOther was null or set to a valid appointment but is now invalid (no longer in the database) then a warning message will display
		///to the user and RefreshAppts will be invoked in order to remove the invalid appt from the UI.</summary>
		public bool IsSelectedApptOtherNull() {
			int idxSelected=gridMain.GetSelectedIndex();
			Appointment appointment=null;
			if(idxSelected!=-1) {
				appointment=Appointments.GetOneApt(ListApptOthers[idxSelected].AptNum);
			}
			if(idxSelected==-1 || appointment==null) {
				MsgBox.Show(this,"Selected appointment no longer exists.");
				RefreshData(PatientCur,null);
				RefreshView();
				return true;
			}
			return false;
		}

		private void FillGrid() {
			gridMain.SetScaleAndZoom(LayoutManager.GetScaleMS(),LayoutManager.GetZoomLocal());
			long selectedApptOtherNum=-1;
			ApptOther appointmentOtherSelected=ListApptOthers.Find(x=>x.AptNum==gridMain.SelectedTag<long>());
			if(appointmentOtherSelected != null){
				selectedApptOtherNum=appointmentOtherSelected.AptNum;
			}
			int selectedIndex=-1;
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormApptsOther","Appt Status"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormApptsOther","Prov"),50);
			gridMain.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g("FormApptsOther","Clinic"),80);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g("FormApptsOther","Date"),70);//If the order changes, reflect the change for dateIndex below.
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormApptsOther","Time"),70);//Must immediately follow Date column.
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormApptsOther","Min"),40);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormApptsOther","Procedures"),150);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormApptsOther","Notes"),320);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			int idxDate=3;
			if(!PrefC.HasClinicsEnabled) {
				idxDate=2;
			}
			for(int i=0;i<ListApptOthers.Count;i++) {
				row=new GridRow();
				row.Cells.Add(ListApptOthers[i].AptStatus.ToString());
				row.Cells.Add(Providers.GetAbbr(ListApptOthers[i].ProvNum));
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(ListApptOthers[i].ClinicNum));
				}
				row.Cells.Add("");//Date
				row.Cells.Add("");//Time
				if(ListApptOthers[i].AptDateTime.Year > 1880) {
					//only regular still scheduled appts
					if(ListApptOthers[i].AptStatus!=ApptStatus.Planned && ListApptOthers[i].AptStatus!=ApptStatus.PtNote 
						&& ListApptOthers[i].AptStatus!=ApptStatus.PtNoteCompleted && ListApptOthers[i].AptStatus!=ApptStatus.UnschedList 
						&& ListApptOthers[i].AptStatus!=ApptStatus.Broken) 
					{
						row.Cells[idxDate].Text=ListApptOthers[i].AptDateTime.ToString("d");
						row.Cells[idxDate+1].Text=ListApptOthers[i].AptDateTime.ToString("t");
						if(ListApptOthers[i].AptDateTime < DateTime.Today) { //Past
							row.ColorBackG=_listDefsProgNoteColors[11].ItemColor;
							row.ColorText=_listDefsProgNoteColors[10].ItemColor;
						}
						else if(ListApptOthers[i].AptDateTime.Date==DateTime.Today.Date) { //Today
							row.ColorBackG=_listDefsProgNoteColors[9].ItemColor;
							row.ColorText=_listDefsProgNoteColors[8].ItemColor;
							row.Cells[0].Text=Lan.g(this,"Today");
						}
						else if(ListApptOthers[i].AptDateTime > DateTime.Today) { //Future
							row.ColorBackG=_listDefsProgNoteColors[13].ItemColor;
							row.ColorText=_listDefsProgNoteColors[12].ItemColor;
						}
					}
					else if(ListApptOthers[i].AptStatus==ApptStatus.Planned) { //show line for planned appt
						row.ColorBackG=_listDefsProgNoteColors[17].ItemColor;
						row.ColorText=_listDefsProgNoteColors[16].ItemColor;
						string strText=Lan.g("enumApptStatus","Planned")+" ";
						int idxPlannedApt=_listPlannedApptsIncomplete.FindIndex(x => x.AptNum==ListApptOthers[i].AptNum);
						if(IsShowCompletePlanned) {
							for(int p=0;p<_listPlannedAppts.Count;p++) {
								if(_listPlannedAppts[p].AptNum==ListApptOthers[i].AptNum) {
									strText+="#"+_listPlannedAppts[p].ItemOrder.ToString();
								}
							}
						}
						else {
							if(idxPlannedApt>=0) {
								strText+="#"+(idxPlannedApt+1);
							}
							else {
								continue;
							}
						}
						if(idxPlannedApt<0) {//attached to a completed appointment
							strText+=" ("+Lan.g("enumApptStatus",ApptStatus.Complete.ToString())+")";
						}
						if(ListApptOthers.FindAll(x => x.NextAptNum==ListApptOthers[i].AptNum)
							.Exists(x => x.AptStatus==ApptStatus.Scheduled)) //attached to a scheduled appointment
						{
							strText+=" ("+Lan.g("enumApptStatus",ApptStatus.Scheduled.ToString())+")";
						}
						row.Cells[0].Text=strText;
					}
					else if(ListApptOthers[i].AptStatus==ApptStatus.PtNote) {
						row.ColorBackG=_listDefsProgNoteColors[19].ItemColor;
						row.ColorText=_listDefsProgNoteColors[18].ItemColor;
						row.Cells[0].Text=Lan.g("enumApptStatus","PtNote");
					}
					else if(ListApptOthers[i].AptStatus==ApptStatus.PtNoteCompleted) {
						row.ColorBackG=_listDefsProgNoteColors[21].ItemColor;
						row.ColorText=_listDefsProgNoteColors[20].ItemColor;
						row.Cells[0].Text=Lan.g("enumApptStatus","PtNoteCompleted");
					}
					else if(ListApptOthers[i].AptStatus==ApptStatus.Broken) {
						row.Cells[0].Text=Lan.g("enumApptStatus","Broken");
						row.Cells[idxDate].Text=ListApptOthers[i].AptDateTime.ToString("d");
						row.Cells[idxDate+1].Text=ListApptOthers[i].AptDateTime.ToString("t");
						row.ColorBackG=_listDefsProgNoteColors[15].ItemColor;
						row.ColorText=_listDefsProgNoteColors[14].ItemColor;
					}
					else if(ListApptOthers[i].AptStatus==ApptStatus.UnschedList) {
						row.Cells[0].Text=Lan.g("enumApptStatus","UnschedList");
						row.ColorBackG=_listDefsProgNoteColors[15].ItemColor;
						row.ColorText=_listDefsProgNoteColors[14].ItemColor;
					}
				}
				row.Cells.Add((ListApptOthers[i].Pattern.Length * 5).ToString());
				row.Cells.Add(ListApptOthers[i].ProcDescript);
				row.Cells.Add(ListApptOthers[i].Note);
				row.Tag=ListApptOthers[i].AptNum;
				gridMain.ListGridRows.Add(row);
				if((long)row.Tag==selectedApptOtherNum) {
					//we will not use i as the index because there are "continue"s in this loop
					selectedIndex=gridMain.ListGridRows.Count-1;//select the row that was just added if it matches
				}
			}
			gridMain.EndUpdate();
			if(selectedIndex>-1) {
				gridMain.SetSelected(selectedIndex,true);
			}
		}
	}

	public class PinBoardArgs {
		public ApptOther ApptOther_;
		public List<ApptOther> ListApptOthers;
		public Patient Patient_;

		public PinBoardArgs(Patient patient,ApptOther apptOther,List<ApptOther> listApptOthers) {
			Patient_=patient;
			ApptOther_=apptOther;
			ListApptOthers=listApptOthers;
		}
	}
}
