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
		private bool IsTriage;
		private FormWindowState windowStateOld;

		public UserControlTasksTab TaskTab {
			get {
				return userControlTasks1.TaskTab;
			}
			set {
				userControlTasks1.TaskTab=value;
			}
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
			windowStateOld=WindowState;
			userControlTasks1.InitializeOnStartup();
			splitter.Panel2Collapsed=true;
			LayoutManager.LayoutControlBoundsAndFonts(splitter);
		}

		public override void ProcessSignalODs(List<Signalod> listSignals) {
			if(listSignals.Exists(x => x.IType==InvalidType.WebChatSessions)) {
				FillGridWebChats();
			}
		}
		
		private void userControlTasks1_GoToChanged(object sender,EventArgs e) {
			TaskObjectType gotoType=userControlTasks1.GotoType;
			long gotoKeyNum=userControlTasks1.GotoKeyNum;
			if(gotoType==TaskObjectType.Patient){
				if(gotoKeyNum!=0){
					Patient pat=Patients.GetPat(gotoKeyNum);
					//OnPatientSelected(pat);
					if(IsTriage) {
						GotoModule.GotoChart(pat.PatNum);
					}
					else {
						GotoModule.GotoAccount(pat.PatNum);
					}
				}
			}
			if(gotoType==TaskObjectType.Appointment){
				if(gotoKeyNum!=0){
					Appointment apt=Appointments.GetOneApt(gotoKeyNum);
					if(apt==null){
						MsgBox.Show(this,"Appointment has been deleted, so it's not available.");
						return;
						//this could be a little better, because window has closed, but they will learn not to push that button.
					}
					DateTime dateSelected=DateTime.MinValue;
					if(apt.AptStatus==ApptStatus.Planned || apt.AptStatus==ApptStatus.UnschedList){
						//I did not add feature to put planned or unsched apt on pinboard.
						MsgBox.Show(this,"Cannot navigate to appointment.  Use the Other Appointments button.");
						//return;
					}
					else{
						dateSelected=apt.AptDateTime;
					}
					Patient pat=Patients.GetPat(apt.PatNum);
					//OnPatientSelected(pat);
					GotoModule.GotoAppointment(dateSelected,apt.AptNum);
				}
			}
			//DialogResult=DialogResult.OK;
		}

		///<summary>Used by OD HQ.</summary>
		public void ShowTriage() {
			userControlTasks1.Width=gridWebChatSessions.Left-3;
			splitter.Panel2Collapsed=false;
			LayoutManager.LayoutControlBoundsAndFonts(splitter);
			userControlTasks1.FillGridWithTriageList();//always show webchat panel when HQ - triage
			FillGridWebChats();
			IsTriage=true;
		}

		///<summary>Only for ODHQ triage.</summary>
		private void FillGridWebChats() {
			if(splitter.Panel2Collapsed) {
				return;
			}
			gridWebChatSessions.BeginUpdate();
			gridWebChatSessions.ListGridRows.Clear();
			if(gridWebChatSessions.ListGridColumns.Count==0) {
				gridWebChatSessions.ListGridColumns.Add(new GridColumn("DateTime",80,HorizontalAlignment.Center));
				gridWebChatSessions.ListGridColumns.Add(new GridColumn("IsEnded",60,HorizontalAlignment.Center));
				gridWebChatSessions.ListGridColumns.Add(new GridColumn("Owner",80,HorizontalAlignment.Left));
				gridWebChatSessions.ListGridColumns.Add(new GridColumn("PatNum",80,HorizontalAlignment.Right));
				gridWebChatSessions.ListGridColumns.Add(new GridColumn("SessionNum",90,HorizontalAlignment.Right));
				gridWebChatSessions.ListGridColumns.Add(new GridColumn("Question",90,HorizontalAlignment.Left){ IsWidthDynamic=true });
			}
			List <WebChatSession> listChatSessions=null;
			List <WebChatMessage> listChatMessages=null;
			//If connection to webchat is lost or not visible from a specific network location, then continue, in order to keep the call center operational.
			ODException.SwallowAnyException(() => {
				listChatSessions=WebChatSessions.GetSessions(false,DateTime.Now.AddDays(-7),DateTime.Now.AddYears(1));
				listChatMessages=WebChatMessages.GetAllForSessions(listChatSessions.Select(x => x.WebChatSessionNum).ToArray());
			});
			if(listChatSessions!=null) {//Will only be null if connection to webchat database failed.
				foreach(WebChatSession webChatSession in listChatSessions) {
					bool isRelevantSession=false;
					if(String.IsNullOrEmpty(webChatSession.TechName)) {
						isRelevantSession=true;//Unclaimed web chat sessions are visible to all technicians, so they can consider taking ownership.
					}
					else if(webChatSession.TechName==Security.CurUser.UserName) {
						isRelevantSession=true;
					}
					else if(listChatMessages.Exists(x => x.WebChatSessionNum==webChatSession.WebChatSessionNum && x.UserName==Security.CurUser.UserName)) {
						isRelevantSession=true;
					}
					if(!isRelevantSession) {
						continue;
					}
					List <string> listMessagesForSession=listChatMessages
						.Where(x => x.WebChatSessionNum==webChatSession.WebChatSessionNum)
						.Select(x => x.MessageText.ToLower())
						.ToList();
					GridRow row=new GridRow();
					row.Tag=webChatSession;
					row.Cells.Add(webChatSession.DateTcreated.ToString());
					row.Cells.Add((webChatSession.DateTend.Year > 1880)?"X":"");
					TimeSpan spanWebChatAge=new TimeSpan(DateTime.Now.Ticks-webChatSession.DateTcreated.Ticks);
					Color backgroundColor=Color.FromArgb(247,110,110); //Changing this color will also need to change color in FormWebChatTools.cs
					if(spanWebChatAge.TotalMinutes>2) {
						//Change the color to red if the time since it was created is greater than 2 minutes.
						backgroundColor=Color.Red;
					}
					if(string.IsNullOrEmpty(webChatSession.TechName)) {
						row.Cells.Add("NEEDS TECH");
						row.Bold=true;
						row.ColorBackG=backgroundColor;
						row.ColorText=Color.White;
					}
					else {
						row.Cells.Add(webChatSession.TechName);
					}
					row.Cells.Add((webChatSession.PatNum==0)?"":webChatSession.PatNum.ToString());
					row.Cells.Add(webChatSession.WebChatSessionNum.ToString());
					row.Cells.Add(webChatSession.QuestionText);
					gridWebChatSessions.ListGridRows.Add(row);
				}
			}
			gridWebChatSessions.EndUpdate();
		}

		private void gridWebChatSessions_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			FormWebChatSession form=new FormWebChatSession((WebChatSession)gridWebChatSessions.ListGridRows[e.Row].Tag,
				() => {
					FillGridWebChats();//Refresh to show if session ended or if tech has been assigned to the session.
				});
			form.Show();
		}

		private void userControlTasks1_Resize(object sender,EventArgs e) {
			if(WindowState==FormWindowState.Minimized) {//Form currently minimized.
				windowStateOld=WindowState;
				return;//The window is invisble when minimized, so no need to refresh.
			}
			if(windowStateOld==FormWindowState.Minimized) {//Form was previously minimized (invisible) and is now in normal state or maximized state.
				FillGridWebChats();//Refresh the grid height because the form height might have changed.
				windowStateOld=WindowState;
				return;
			}
			windowStateOld=WindowState;//Set the window state after every resize.
		}

		private void UserControlTasks1_FillGridEvent(object sender,EventArgs e) {
			this.Text=userControlTasks1.ControlParentTitle;
		}

		private void splitter_SplitterMoved(object sender, SplitterEventArgs e){
			LayoutManager.LayoutControlBoundsAndFonts(splitter);
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





















