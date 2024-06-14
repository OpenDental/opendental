using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormTasks:FormODBase {
		//private System.ComponentModel.IContainer components;
		/////<summary>After closing, if this is not zero, then it will jump to the object specified in GotoKeyNum.</summary>
		//public TaskObjectType GotoType;
		/////<summary>After closing, if this is not zero, then it will jump to the specified patient.</summary>
		//public long GotoKeyNum;
		private bool _isTriage;
		private FormWindowState _formWindowStateOld;
		public static Color ColorLightRed=Color.FromArgb(247,110,110);

		public void SetUserControlTasksTab(UserControlTasksTab newTab){
			userControlTasks1.TaskTab=newTab;
		}

		///<summary></summary>
		public FormTasks()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			//Lan.F(this);
		}

		private void FormTasks_Load(object sender,EventArgs e) {
			_formWindowStateOld=WindowState;
			userControlTasks1.InitializeOnStartup();
			splitContainer.Panel2Collapsed=true;
		}

		public override void ProcessSignalODs(List<Signalod> listSignalods) {
			if(listSignalods.Exists(x => x.IType==InvalidType.WebChatSessions)) {
				FillGridWebChats();
			}
		}

		private void userControlTasks1_GoToChanged(object sender,EventArgs e) {
			TaskObjectType taskObjectTypeGoto=userControlTasks1.TaskObjectTypeGoTo;
			long gotoKeyNum=userControlTasks1.KeyNumGoTo;
			if(taskObjectTypeGoto==TaskObjectType.Patient){
				if(gotoKeyNum!=0){
					Patient patient=Patients.GetPat(gotoKeyNum);
					//OnPatientSelected(pat);
					if(_isTriage) {
						GlobalFormOpenDental.GotoChart(patient.PatNum);
					}
					else {
						GlobalFormOpenDental.GotoAccount(patient.PatNum);
					}
				}
			}
			if(taskObjectTypeGoto!=TaskObjectType.Appointment){
				return;
			}
			if(gotoKeyNum==0){
				return;
			}
			Appointment appointment=Appointments.GetOneApt(gotoKeyNum);
			if(appointment==null){
				MsgBox.Show(this,"Appointment has been deleted, so it's not available.");
				return;
				//this could be a little better, because window has closed, but they will learn not to push that button.
			}
			DateTime dateTimeSelected=DateTime.MinValue;
			if(appointment.AptStatus==ApptStatus.Planned || appointment.AptStatus==ApptStatus.UnschedList){
				//I did not add feature to put planned or unsched apt on pinboard.
				MsgBox.Show(this,"Cannot navigate to appointment.  Use the Other Appointments button.");
				//return;
			}
			else{
				dateTimeSelected=appointment.AptDateTime;
			}
			//Patient patient=Patients.GetPat(appointment.PatNum);
			//OnPatientSelected(pat);
			GlobalFormOpenDental.GotoAppointment(dateTimeSelected,appointment.AptNum);
			//DialogResult=DialogResult.OK;
		}

		///<summary>Used by OD HQ.</summary>
		public void ShowTriage() {
			userControlTasks1.Width=gridWebChatSessions.Left-3;
			splitContainer.Panel2Collapsed=false;
			userControlTasks1.FillGridWithTriageList();//always show webchat panel when HQ - triage
			FillGridWebChats();
			_isTriage=true;
		}

		///<summary>Only for ODHQ triage.</summary>
		private void FillGridWebChats() {
			if(splitContainer.Panel2Collapsed) {
				return;
			}
			gridWebChatSessions.BeginUpdate();
			gridWebChatSessions.ListGridRows.Clear();
			if(gridWebChatSessions.Columns.Count==0) {
				gridWebChatSessions.Columns.Add(new GridColumn("DateTime",80,HorizontalAlignment.Center));
				gridWebChatSessions.Columns.Add(new GridColumn("IsEnded",60,HorizontalAlignment.Center));
				gridWebChatSessions.Columns.Add(new GridColumn("Owner",80,HorizontalAlignment.Left));
				gridWebChatSessions.Columns.Add(new GridColumn("PatNum",80,HorizontalAlignment.Right));
				gridWebChatSessions.Columns.Add(new GridColumn("SessionNum",90,HorizontalAlignment.Right));
				gridWebChatSessions.Columns.Add(new GridColumn("Question",90,HorizontalAlignment.Left){ IsWidthDynamic=true });
			}
			List <WebChatSession> listWebChatSessions=null;
			List <WebChatMessage> listWebChatMessages=null;
			//If connection to webchat is lost or not visible from a specific network location, then continue, in order to keep the call center operational.
			ODException.SwallowAnyException(() => {
				listWebChatSessions=WebChatSessions.GetSessions(hasEndedSessionsIncluded:false,dateCreatedFrom:DateTime.Now.AddDays(-7),dateCreatedTo:DateTime.Now.AddYears(1),WebChatSessionType.WebSupport);
				listWebChatMessages=WebChatMessages.GetAllForSessions(listWebChatSessions.Select(x => x.WebChatSessionNum).ToArray());
			});
			if(listWebChatSessions!=null) {//Will only be null if connection to webchat database failed.
				for(int i=0;i<listWebChatSessions.Count();i++){
					bool isRelevantSession=false;
					if(String.IsNullOrEmpty(listWebChatSessions[i].TechName)) {
						isRelevantSession=true;//Unclaimed web chat sessions are visible to all technicians, so they can consider taking ownership.
					}
					else if(listWebChatSessions[i].TechName==Security.CurUser.UserName) {
						isRelevantSession=true;
					}
					else if(listWebChatMessages.Exists(x => x.WebChatSessionNum==listWebChatSessions[i].WebChatSessionNum && x.UserName==Security.CurUser.UserName)) {
						isRelevantSession=true;
					}
					if(!isRelevantSession) {
						continue;
					}
					List <string> listMessagesForSession=listWebChatMessages
						.Where(x => x.WebChatSessionNum==listWebChatSessions[i].WebChatSessionNum)
						.Select(x => x.MessageText.ToLower())
						.ToList();
					GridRow row=new GridRow();
					row.Tag=listWebChatSessions[i];
					row.Cells.Add(listWebChatSessions[i].DateTcreated.ToString());
					row.Cells.Add((listWebChatSessions[i].DateTend.Year > 1880)?"X":"");
					TimeSpan timeSpanWebChatAge=new TimeSpan(DateTime.Now.Ticks-listWebChatSessions[i].DateTcreated.Ticks);
					Color colorBackground=ColorLightRed;
					if(timeSpanWebChatAge.TotalMinutes>2) {
						//Change the color to red if the time since it was created is greater than 2 minutes.
						colorBackground=Color.Red;
					}
					if(string.IsNullOrEmpty(listWebChatSessions[i].TechName)) {
						row.Cells.Add("NEEDS TECH");
						row.Bold=true;
						row.ColorBackG=colorBackground;
						row.ColorText=Color.White;
					}
					else {
						row.Cells.Add(listWebChatSessions[i].TechName);
					}
					row.Cells.Add((listWebChatSessions[i].PatNum==0)?"":listWebChatSessions[i].PatNum.ToString());
					row.Cells.Add(listWebChatSessions[i].WebChatSessionNum.ToString());
					row.Cells.Add(listWebChatSessions[i].QuestionText);
					gridWebChatSessions.ListGridRows.Add(row);
				}
			}
			gridWebChatSessions.EndUpdate();
		}

		private void gridWebChatSessions_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Action actionWhenClose=new Action(()=>{
				FillGridWebChats();//Refresh to show if session ended or if tech has been assigned to the session.
			});
			FormWebChatSession formWebChatSession=new FormWebChatSession((WebChatSession)gridWebChatSessions.ListGridRows[e.Row].Tag, actionWhenClose);
			formWebChatSession.Show();
		}

		private void userControlTasks1_Resize(object sender,EventArgs e) {
			if(WindowState==FormWindowState.Minimized) {//Form currently minimized.
				_formWindowStateOld=WindowState;
				return;//The window is invisble when minimized, so no need to refresh.
			}
			if(_formWindowStateOld==FormWindowState.Minimized) {//Form was previously minimized (invisible) and is now in normal state or maximized state.
				FillGridWebChats();//Refresh the grid height because the form height might have changed.
				_formWindowStateOld=WindowState;
				return;
			}
			_formWindowStateOld=WindowState;//Set the window state after every resize.
		}

		private void UserControlTasks1_FillGridEvent(object sender,EventArgs e) {
			this.Text=userControlTasks1.TitleControlParent;
		}

		/* private void timer1_Tick(object sender,EventArgs e) {
				if(Security.CurUser!=null) {//Possible if OD auto logged a user off and they left the task window open in the background.
					userControlTasks1.RefreshTasks();
				}
				//this quick and dirty refresh is not as intelligent as the one used when tasks are docked.
				//Sound notification of new task is controlled from main form completely
				//independently of this visual refresh.
			}
		}
		*/














	}
}





















